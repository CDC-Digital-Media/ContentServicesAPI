// Copyright [2015] [Centers for Disease Control and Prevention] 
// Licensed under the CDC Custom Open Source License 1 (the 'License'); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
// 
//   http://t.cdc.gov/O4O
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Gov.Hhs.Cdc.Logging
{
    public static class Logger
    {
        public static ILog Log
        {
            get
            {
                var host = Dns.GetHostName();
                if (host.ToLower() != Environment.MachineName.ToLower())
                {
                    GlobalContext.Properties["ServerName"] = host + ":" + Environment.MachineName;
                }
                else
                {
                    GlobalContext.Properties["ServerName"] = host;
                }
                log4net.Util.SystemInfo.NullText = string.Empty;
                return LogManager.GetLogger(typeof(Logger));
            }
        }

        public static void LogInfo(string message)
        {
            Log.Info(message);
        }

        public static void LogInfo(string message, string query)
        {
            ThreadContext.Properties["Query"] = query;
            Log.Info(message);
        }

        public static void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public static void LogWarning(string message)
        {
            Log.Warn(message);
        }

        public static void LogError(string message)
        {
            Log.Error(message);
        }

        public static void LogFatal(string message)
        {
            Log.Fatal(message);
        }

        public static void LogError(Exception exception)
        {
            Log.Error(exception.Message, exception);
        }

        public static void LogError(Exception exception, string message)
        {
            Log.Error(message, exception);
        }

        public static void LogError(Exception exception, string message, string query)
        {
            ThreadContext.Properties["Query"] = query;
            Log.Error(message, exception);
        }

        public static void LogError(string message, string query)
        {
            ThreadContext.Properties["Query"] = query;
            Log.Error(message);
        }
    }
}
