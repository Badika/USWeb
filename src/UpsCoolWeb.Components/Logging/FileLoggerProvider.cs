using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace UpsCoolWeb.Components.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private ILogger Logger { get; }

        public FileLoggerProvider(IConfiguration config)
        {
            LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), config["Logging:File:LogLevel:Default"]);
            String path = Path.Combine(config["Application:Path"], config["Logging:File:Path"]);
            Int64 rollSize = Int64.Parse(config["Logging:File:RollSize"]);

            Logger = new FileLogger(path, logLevel, rollSize);
        }

        public ILogger CreateLogger(String categoryName)
        {
            return Logger;
        }

        public void Dispose()
        {
        }
    }
}
