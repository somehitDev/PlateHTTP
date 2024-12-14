using System;
using System.Collections.Generic;
using System.Net;


namespace PlateHTTP.Logging {
    public class Logger: Interfaces.Logging.ILogger {
        public string LoggerName { get; private set; }
        public string MinimumLogLevel { get; private set; }
        public string LoggingFormat { get; set; }
        public Dictionary<string, int> LoggingLevelMap { get; set; }
        public Dictionary<string, ConsoleColor> LoggingLevelColorMap { get; set; }

        public Logger(string loggerName, string minimumLogLevel) {
            this.LoggerName = loggerName;
            this.MinimumLogLevel = minimumLogLevel;
            this.LoggingFormat = $"[{DateTime.Now:yyyy-MM-ddTHH:mm:ss}] {loggerName}::$logLevel - $message";

            this.LoggingLevelMap = new Dictionary<string, int>(){
                { "debug", 0 },
                { "info", 1 },
                { "warn", 2 },
                { "error", 3 }
            };
            this.LoggingLevelColorMap = new Dictionary<string, ConsoleColor>(){
                { "debug", ConsoleColor.Gray },
                { "info", ConsoleColor.Green },
                { "warn", ConsoleColor.Yellow },
                { "error", ConsoleColor.Red }
            };
        }

        public void Log(string logLevel, string logMessage) {
            if (this.LoggingLevelMap[logLevel.ToLower()] >= this.LoggingLevelMap[this.MinimumLogLevel]) {
                Console.ForegroundColor = this.LoggingLevelColorMap[logLevel.ToLower()];
                string loggingMessage = this.LoggingFormat.Replace("$logLevel", logLevel.ToLower()).Replace("$message", logMessage);

                Console.WriteLine(loggingMessage);
                Console.ResetColor();
            }
        }
        public void Debug(string logMessage) {
            this.Log("debug", logMessage);
        }
        public void Info(string logMessage) {
            this.Log("info", logMessage);
        }
        public void Warn(string logMessage) {
            this.Log("warn", logMessage);
        }
        public void Error(string logMessage) {
            this.Log("error", logMessage);
        }
        public void Except(string logMessage) {
            this.Error(logMessage);
            throw new WebException(logMessage);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
