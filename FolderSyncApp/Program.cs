using CommandLine;
using NLog;
using NLog.Targets;

namespace FolderSyncApp
{
    internal class Program
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                UpdateLogPath(o.LogsPath);
                logger.Info("Loaded configuration {@o}", o);
                if(ParamsCheck(o))
                {
                    
                }   
            });
        }


        private static void UpdateLogPath(string path)
        {
            var target = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            target.FileName = Path.Combine(path, "logs.txt");
            LogManager.ReconfigExistingLoggers();
        }

        private static bool ParamsCheck(Options options)
        {
            if(!Directory.Exists(options.SourceFolder))
            {
                logger.Error($"Source folder {options.SourceFolder} not found");
                return false;
            }
            if(!Directory.Exists(options.TargetFolder))
            {
                logger.Error($"Target folder {options.TargetFolder} not found");
                return false;
            }
            if(!Directory.Exists(options.LogsPath))
            {
                logger.Error($"Log folder {options.LogsPath} not found");
                return false;
            }
            return true;
        }
    }
}
