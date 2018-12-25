using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using UpsCoolWeb.Components.Extensions;
using System;
using System.IO;
using System.Text;

namespace UpsCoolWeb.Components.Logging
{
    public class FileLogger : ILogger
    {
        private Int64 RollSize { get; }
        private String LogPath { get; }
        private LogLevel Level { get; }
        private String LogDirectory { get; }
        private Func<Int32?> AccountId { get; }
        private String RollingFileFormat { get; }
        private static Object LogWriting { get; } = new Object();

        public FileLogger(String path, LogLevel logLevel, Int64 rollSize)
        {
            IHttpContextAccessor accessor = new HttpContextAccessor();
            String file = Path.GetFileNameWithoutExtension(path);
            String extension = Path.GetExtension(path);
            LogDirectory = Path.GetDirectoryName(path);

            RollingFileFormat = $"{file}-{{0:yyyyMMdd-HHmmss}}{extension}";
            AccountId = () => accessor.HttpContext?.User.Id();
            RollSize = rollSize;
            Level = logLevel;
            LogPath = path;
        }

        public Boolean IsEnabled(LogLevel logLevel)
        {
            return Level <= logLevel;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, String> formatter)
        {
            if (IsEnabled(logLevel))
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine($"{logLevel.ToString().PadRight(11)}: {DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff} [{AccountId()}]");
                log.AppendLine($"Message    : {formatter(state, exception)}");
                AppendStackTrace(log, exception);
                log.AppendLine();

                lock (LogWriting)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(LogPath, log.ToString());

                    if (RollSize <= new FileInfo(LogPath).Length)
                        File.Move(LogPath, Path.Combine(LogDirectory, String.Format(RollingFileFormat, DateTime.Now)));
                }
            }
        }

        private void AppendStackTrace(StringBuilder log, Exception exception)
        {
            if (exception != null)
                log.AppendLine("Stack trace:");

            while (exception != null)
            {
                log.AppendLine($"    {exception.GetType()}: {exception.Message}");
                foreach (String line in exception.StackTrace.Split('\n'))
                    log.AppendLine("     " + line.TrimEnd('\r'));

                exception = exception.InnerException;
            }
        }
    }
}
