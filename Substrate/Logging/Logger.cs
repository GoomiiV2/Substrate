using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct3D11.Debug;

namespace Substrate.Logging
{
    public static partial class LogCat
    {
        public static readonly int Default   = Logger.RegisterCategory("Default");
        public static readonly int Substrate = Logger.RegisterCategory("Substrate");
    }

    public class Logger
    {
        public static HashSet<string> Categories { get; private set; } = new HashSet<string>();
        public static bool LogToFiles = false;

        public static int RegisterCategory(string name)
        {
            if (Categories.Add(name))
                return Categories.Count - 1;
            else
                return -1;
        }

        public Logger()
        {
            var logConfig = new LoggerConfiguration();
            logConfig.MinimumLevel.Verbose();
            logConfig.WriteTo.Console(outputTemplate: "[{Level:u3}] [{Category}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information);

#if DEBUG
            logConfig.WriteTo.Debug();
#endif

            if (Substrate.Config.EnableFileLogging)
            {
                var logTemplate = "{Timestamp:HH:mm:ss.fff} [{Level:u3}] [{Category}] {Message:lj}{NewLine}{Exception}";
                logConfig.WriteTo.File(Path.Combine(Substrate.Config.LogsDir, "log.txt"), outputTemplate: logTemplate, shared: true);

                var lastRunLogPath = Path.Combine(Substrate.Config.LogsDir, "last_run.txt");
                if (File.Exists(lastRunLogPath))
                    File.Delete(lastRunLogPath);

                logConfig.WriteTo.File(lastRunLogPath, outputTemplate: logTemplate, shared: true);
            }


            Serilog.Log.Logger = logConfig.CreateLogger();
        }

        public void Log(int category, LogEventLevel level, string msg, params object[] args)
        {
            var isCatValid = category >= 0 && category < Categories.Count();
            var catName    = Categories.ElementAt(isCatValid ? category : LogCat.Default);
            Serilog.Log.ForContext("Category", catName).Write(level, msg, args);
        }

        public void Info(int category, string msg, params object[] args) => Log(category, LogEventLevel.Information, msg, args);
        public void Warn(int category, string msg, params object[] args) => Log(category, LogEventLevel.Warning, msg, args);
        public void Error(int category, string msg, params object[] args) => Log(category, LogEventLevel.Error, msg, args);
        public void Trace(int category, string msg, params object[] args) => Log(category, LogEventLevel.Verbose, msg, args);
    }
}
