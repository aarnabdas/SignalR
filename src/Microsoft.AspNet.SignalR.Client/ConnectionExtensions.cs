// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;

namespace Microsoft.AspNet.SignalR.Client
{
    public static class ConnectionExtensions
    {
        public static T GetValue<T>(this IConnection connection, string key)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            lock (connection.Items)
            {
                object value;
                if (connection.Items.TryGetValue(key, out value))
                {
                    return (T)value;
                }
            }

            return default(T);
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "jsonWriter will not dispose the stringWriter")]
        public static string JsonSerializeObject(this IConnection connection, object value)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            var sb = new StringBuilder(0x100);
            using (var stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter) { CloseOutput = false })
                {
                    jsonWriter.Formatting = connection.JsonSerializer.Formatting;
                    connection.JsonSerializer.Serialize(jsonWriter, value);
                }

                return stringWriter.ToString();
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "jsonTextReader will not dispose the stringReader")]
        public static T JsonDeserializeObject<T>(this IConnection connection, string jsonValue)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            using (var stringReader = new StringReader(jsonValue))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader) { CloseInput = false })
                {
                    return (T)connection.JsonSerializer.Deserialize(jsonTextReader, typeof(T));
                }
            }
        }

        public static string XmlSerializeObject<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var xmlserializer = new XmlSerializer(typeof(T));
                var settings = new XmlWriterSettings() {
                    OmitXmlDeclaration = true
                };
                using (var stringWriter = new StringWriter())
                {
                    using (var writer = XmlWriter.Create(stringWriter, settings))
                    {
                        xmlserializer.Serialize(writer, value, emptyNamepsaces);
                        return stringWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static bool EnsureReconnecting(this IConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (connection.ChangeState(ConnectionState.Connected, ConnectionState.Reconnecting))
            {
                connection.OnReconnecting();
            }

            return connection.State == ConnectionState.Reconnecting;
        }

#if !PORTABLE && !__ANDROID__ && !IOS
        public static IObservable<string> AsObservable(this Connection connection)
        {
            return connection.AsObservable(value => value);
        }

        public static IObservable<T> AsObservable<T>(this Connection connection)
        {
            return connection.AsObservable(value => connection.JsonDeserializeObject<T>(value));
        }

        public static IObservable<T> AsObservable<T>(this Connection connection, Func<string, T> selector)
        {
            return new ObservableConnection<T>(connection, selector);
        }
#endif
    }
}
