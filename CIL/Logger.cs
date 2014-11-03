using System;
using System.Diagnostics;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace WpfMdiApp1.CIL
{
    #region | Interface |

    public interface ILogger
    {
        bool IsDebugEnabled { get; }

        void Info(string message);

        void Trace(string message);

        void Debug(string message);

        void Warn(string message);

        void Error(string message, Exception exception);

        void Fatal(string message, Exception exception);
    }

    #endregion

    public class Logger : ILogger
    {
        #region | Fields |

        private readonly NLog.Logger _logger;

        #endregion

        #region | Properties |

        public bool IsDebugEnabled { get; private set; }

        #endregion

        #region | Constructor |

        public Logger()
        {
            LoggerConfig();

            _logger = LogManager.GetCurrentClassLogger();

            IsDebugEnabled = _logger.IsDebugEnabled;

            _logger.Debug("======================================================");
        }

        #endregion

        #region | Methods |

        public void Info(string message)
        {
            //StackFrame frame = new StackFrame(1);
            //MethodBase method = frame.GetMethod();

            //if (method.DeclaringType != null)
            //    message = string.Format("{0} {1}", method.DeclaringType.FullName, message);
            //else
            //    message = string.Format("{0} {1}", "...not resolved...", message); 

            _logger.Info(message);
        }

        public void Trace(string message)
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();

            if (method.DeclaringType != null)
                message = string.Format("{0} {1}", method.DeclaringType.FullName, message);
            else
                message = string.Format("{0} {1}", "...not resolved...", message);

            _logger.Trace(message);
        }

        public void Debug(string message)
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();

            if (method.DeclaringType != null)
                message = string.Format("{0} {1}", method.DeclaringType.FullName, message);
            else
                message = string.Format("{0} {1}", "...not resolved...", message);

            _logger.Debug(message);
        }

        public void Warn(string message)
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();

            if (method.DeclaringType != null)
                message = string.Format("{0} {1}", method.DeclaringType.FullName, message);
            else
                message = string.Format("{0} {1}", "...not resolved...", message);

            _logger.Warn(message);
        }

        public void Error(string message, Exception exception)
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();

            if (method.DeclaringType != null)
                message = string.Format("{0} {1} {2}", method.DeclaringType.FullName, message, exception.Message);
            else
                message = string.Format("{0} {1} {2}", "...not resolved...", message, exception.Message);

            _logger.Error(message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();

            if (method.DeclaringType != null)
                message = string.Format("{0} {1} {2}", method.DeclaringType.FullName, message, exception.Message);
            else
                message = string.Format("{0} {1} {2}", "...not resolved...", message, exception.Message);

            _logger.Fatal(message, exception);
        }

        private void LoggerConfig()
        {
            //  Step 0. Check console logiing parameter
            string logLevelConsole = string.Empty;
            bool logToConsole = IoC.Instance.Params.TryGetValue("LogLevelConsole", out logLevelConsole);
            ColoredConsoleTarget consoleTarget = null;
            
            // Step 1. Create configuration object 
            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            if (logToConsole)
            {
                consoleTarget = new ColoredConsoleTarget();
                config.AddTarget("console", consoleTarget);
            }

            // Step 3. Set target properties 
            string app = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", "").Replace(".vshost", "");
            fileTarget.FileName = "${basedir}/" + app + ".log";
            fileTarget.Layout = @"${longdate} ${message}";

            if (logToConsole)
            {
                consoleTarget.Layout = @"${message}";
            }

            // Step 4. Define file rules
            LogLevel fileLev = LogLevel.Warn;

            string logLevelFile;

            if (!IoC.Instance.Params.TryGetValue("LogLevel", out logLevelFile))
                throw new Exception("Не задан уровень логгирования в файл!");
            
            switch (logLevelFile)
            {
                case "Trace":
                    fileLev = LogLevel.Trace;
                    break;
                case "Info":
                    fileLev = LogLevel.Info;
                    break;
                case "Debug":
                    fileLev = LogLevel.Debug;
                    break;
                case "Warn":
                    fileLev = LogLevel.Warn;
                    break;
                case "Error":
                    fileLev = LogLevel.Error;
                    break;
            }
            LoggingRule rule1 = new LoggingRule("*", fileLev, fileTarget);
            config.LoggingRules.Add(rule1);

            // Step 5. Define console rules
            if (logToConsole)
            {
                LogLevel consoleLev = LogLevel.Warn;

                switch (logLevelConsole)
                {
                    case "Trace":
                        consoleLev = LogLevel.Trace;
                        break;
                    case "Info":
                        consoleLev = LogLevel.Info;
                        break;
                    case "Debug":
                        consoleLev = LogLevel.Debug;
                        break;
                    case "Warn":
                        consoleLev = LogLevel.Warn;
                        break;
                    case "Error":
                        consoleLev = LogLevel.Error;
                        break;
                }

                LoggingRule rule2 = new LoggingRule("*", consoleLev, consoleTarget);
                config.LoggingRules.Add(rule2);
            }

            // Step 6. Activate the configuration
            LogManager.Configuration = config;
        }

        #endregion
    }
}
