// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using System.Configuration;
using Microsoft.AspNet.SignalR.Core.ConnectionConfiguration;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Crank
{
    public class Client
    {
        private static CrankArguments Arguments;
        private static ConcurrentBag<Connection> Connections = new ConcurrentBag<Connection>();
        private static ConcurrentDictionary<string, List<Connection>> ConnectionsDictionary = new ConcurrentDictionary<string, List<Connection>>();
        private static ConcurrentBag<IHubProxy> HubProxies = new ConcurrentBag<IHubProxy>();
        private static ControllerEvents TestPhase = ControllerEvents.None;
        private static IConnectionFactory Factory;
        private static List<ConnectionConfigurationElement> ConnectionConfList;

        public static void Main()
        {
            Arguments = CrankArguments.Parse();

            ThreadPool.SetMinThreads(Arguments.Connections, 2);
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            ComposeConnectionFactory();

            ReadConfiguration();
            ConnectionSampler.Init(ConnectionConfList);
            if (Arguments.IsController)
            {
                ControllerHub.Start(Arguments);
            }

            Run().Wait();

            ConnectionSampler.WriteLog(Arguments.LogFile, Arguments.SendTimeout).Wait();
        }

        private static void ComposeConnectionFactory()
        {
            try
            {
                using (var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory))
                using (var container = new CompositionContainer(catalog))
                {
                    var export = container.GetExportedValueOrDefault<IConnectionFactory>();
                    if (export != null)
                    {
                        Factory = export;
                        Console.WriteLine("Using {0}", Factory.GetType());
                    }
                }
            }
            catch (ImportCardinalityMismatchException)
            {
                Console.WriteLine("More than one IConnectionFactory import was found.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (Factory == null)
            {
                Factory = new HubConnectionFatory();
                Console.WriteLine("Using default connection factory...");
            }
        }

        private static async Task Run()
        {
            while (TestPhase != ControllerEvents.Connect)
            {
                if (TestPhase == ControllerEvents.Abort)
                {
                    Console.WriteLine("Test Aborted");
                    return;
                }

                await Task.Delay(CrankArguments.ConnectionPollIntervalMS);
            }

            await RunConnect();
            await RunSend();

            RunDisconnect();
        }

        internal static void OnPhaseChanged(ControllerEvents phase)
        {
            Debug.Assert(phase != ControllerEvents.None);
            Debug.Assert(phase != ControllerEvents.Sample);

            TestPhase = phase;

            if (!Arguments.IsController)
            {
                Console.WriteLine("Running: {0}", Enum.GetName(typeof(ControllerEvents), phase));
            }
        }

        internal static void OnSample(int id)
        {
            var states = Connections.Select(c => c.State);

            var messages = Connections.SelectMany(c => c.SentMessages.OfType<MessageItem>());

            if (messages.Count() > 0)
            {
                //DAL.MessageItemsLayer.PersistSentMessages(new MessageItems()
                //{
                //    Messages = messages.ToArray<MessageItem>()
                //}.XmlSerializeObject<MessageItems>());
                Connections.Select(c => c.ClearSentMessages());
            }
            var statesArr = new int[3]
            {
                states.Where(s => s == ConnectionState.Connected).Count(),
                states.Where(s => s == ConnectionState.Reconnecting).Count(),
                states.Where(s => s == ConnectionState.Disconnected).Count()
            };

            //if (ControllerProxy != null)
            //{
            //    ControllerProxy.Invoke("Mark", id, statesArr);
            //}
            //else
            //{
            ControllerHub.MarkInternal(id, statesArr);
            //}

            if (!Arguments.IsController)
            {
                Console.WriteLine("{0} Connected, {1} Reconnected, {2} Disconnected", statesArr[0], statesArr[1], statesArr[2]);
            }
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.GetBaseException());
            e.SetObserved();
        }

        private static async Task RunSend()
        {
            var payload = (Arguments.SendBytes == 0) ? String.Empty : new string('a', Arguments.SendBytes);

            Thread.Sleep(2500);
            while (TestPhase == ControllerEvents.Send)
            {
                if (!String.IsNullOrEmpty(payload))
                {
                    //await Task.WhenAll(Connections.Select(c => c.Send(new MessageItem()
                    //{
                    //    Name = c.Items["name"] as string
                    //})));
                }

                await Task.Delay(Arguments.SendInterval);
            }
        }

        private static void RunDisconnect()
        {
            if (Connections.Count > 0)
            {
                if ((TestPhase == ControllerEvents.Disconnect) ||
                    (TestPhase == ControllerEvents.Abort))
                {
                    Parallel.ForEach(Connections, c => c.Dispose());
                }
            }
        }

        private static async Task RunConnect()
        {
            await Task.WhenAll(ConnectionConfList.Select(c => Connect(c)));
        }

        private static void ReadConfiguration()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = Arguments.ConfigFile;
                System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                ConnectionConfigurationSection configurationSection = (ConnectionConfigurationSection)config.GetSection("connections.conf");
                if (configurationSection != null && configurationSection.Connections != null && configurationSection.Connections.Count > 0)
                {
                    ConnectionConfList = new List<ConnectionConfigurationElement>();
                    Arguments.Connections = 0;
                    foreach (ConnectionConfigurationElement connConf in configurationSection.Connections)
                    {
                        Arguments.Connections += connConf.Connections;
                        ConnectionConfList.Add(connConf);
                    }
                }
                else
                {
                    throw new ConfigurationErrorsException("No connection specified in the configuration!!!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task Connect(ConnectionConfigurationElement connConf)
        {
            int connectedCount = 0;
            do
            {
                await ConnectBatch(connConf);
                connectedCount = ConnectionsDictionary[connConf.Name].Where(c => c.State == ConnectionState.Connected).Count();
            } while (connectedCount <= connConf.Connections);
        }

        private static async Task ConnectBatch(ConnectionConfigurationElement connConf)
        {
            var tasks = new Task[Arguments.BatchSize];

            for (int i = 0; i < Arguments.BatchSize; i++)
            {
                tasks[i] = ConnectSingle(connConf);
            }
            await Task.WhenAll(tasks);
        }

        private static Task ConnectSingle(ConnectionConfigurationElement connConf)
        {
            var connection = Factory.CreateConnection(connConf.Url, connConf.Proxy, connConf.Channel, connConf.Name);

            if (connection is HubConnectionD)
            {
                (connection as HubConnectionD).MessageReceived = (string connName, string msg) => {
                    ConnectionSampler.AddMessage(connName, msg);
                };
            }

            connection.Closed += () =>
            {
                Connections.TryTake(out connection);
            };

            connection.Items.Add("name", RandomGenerator.Name());

            Connections.Add(connection);

            List<Connection> connectionsList = new List<Connection>();
            if (!ConnectionsDictionary.TryGetValue(connConf.Name, out connectionsList))
            {
                ConnectionsDictionary.TryAdd(connConf.Name, new List<Connection>());
            }
            ConnectionsDictionary[connConf.Name].Add(connection);

            try
            {
                if (Arguments.Transport == null)
                {
                    return connection.Start();
                }
                else
                {
                    return connection.Start(Arguments.GetTransport());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection.Start Failed: {0}: {1}", e.GetType(), e.Message);
                return new Task(() => { throw e; });
            }
        }
    }
}
