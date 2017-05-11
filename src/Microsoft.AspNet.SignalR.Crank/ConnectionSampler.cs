using Microsoft.AspNet.SignalR.Core.ConnectionConfiguration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Crank
{
    public class ConnectionSamplerUnit
    {
        public DateTime TimeInstance { get; set; }
        public string Message { get; set; }

        public ConnectionSamplerUnit(string message)
        {
            this.TimeInstance = DateTime.Now;
            this.Message = message;
        }
    }

    public class ConnectionSampler
    {
        private static DateTime _samplingStartTime;
        private static ConcurrentDictionary<string, List<ConnectionSamplerUnit>> _samplers;

        public static void Init(List<ConnectionConfigurationElement> connConfList)
        {
            _samplingStartTime = DateTime.Now;
            _samplers = new ConcurrentDictionary<string, List<ConnectionSamplerUnit>>();
            foreach (var conf in connConfList)
            {
                _samplers.TryAdd(conf.Name, new List<ConnectionSamplerUnit>());
            }
        }

        public static void AddMessage(string connName, string message)
        {
            _samplers[connName].Add(new ConnectionSamplerUnit(message));
        }

        public static async Task WriteLog(string filePath, int sendTimeout)
        { 
            List<string> content = new List<string>();

            DateTime endSamplingTime = _samplingStartTime.AddSeconds((double)sendTimeout);

            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                await outputFile.WriteLineAsync("Time,Connection Name,Messages Received");
                do{
                    var samplingUpperBound = _samplingStartTime.AddSeconds(1);
                    string line, time = _samplingStartTime.ToString("MM/dd/yyyy HH:ss");
                    foreach (var connName in _samplers.Keys)
                    {
                        line = string.Empty;
                        line += string.Format("{0},{1},{2}", time, connName,
                            _samplers[connName].Where(c => c.TimeInstance > _samplingStartTime && c.TimeInstance <= samplingUpperBound).Count());
                        await outputFile.WriteLineAsync(line);
                    }
                    _samplingStartTime = samplingUpperBound;
                } while (_samplingStartTime <= endSamplingTime);
            }
        }
    }
}
