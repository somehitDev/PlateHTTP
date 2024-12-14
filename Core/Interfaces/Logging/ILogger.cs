using System;
using System.Collections.Generic;



namespace PlateHTTP.Interfaces.Logging {
    public interface ILogger {
        public string LoggerName { get; }
        public string MinimumLogLevel { get; }
        public string LoggingFormat { get; set; }
        public Dictionary<string, int> LoggingLevelMap { get; set; }
        public Dictionary<string, ConsoleColor> LoggingLevelColorMap { get; set; }

        void Log(string logLevel, string logMessage);
        void Debug(string logMessage);
        void Info(string logMessage);
        void Warn(string logMessage);
        void Error(string logMessage);
        void Except(string logMessage);

        void Dispose();
    }
}
