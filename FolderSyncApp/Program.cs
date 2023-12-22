using CommandLine;
using NLog;
using NLog.Targets;
using ThreadingTimer = System.Threading.Timer;

namespace FolderSyncApp
{
    internal class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                UpdateLogPath(o.LogsPath);
                logger.Info("Loaded configuration {@config}",o);
                if (ParamsCheck(o))
                {
                    var worker = new FolderWorker(o.SourceFolder, o.TargetFolder);
                    var timer = new ThreadingTimer(_ => worker.StartScanning(), null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(o.SyncInterval));
                    Console.WriteLine("Running...... for Exit press Enter");
                    Console.ReadLine();

                }
                else
                {
                    Console.WriteLine("Error check log for details, press enter to Exit");
                    Console.ReadLine();
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
            //Check if parameters are valid folders
            if (!Directory.Exists(options.SourceFolder))
            {
                logger.Error($"Source folder {options.SourceFolder} not found or not correct path");
                return false;
            }
            if (!Directory.Exists(options.TargetFolder))
            {
                logger.Error($"Target folder {options.TargetFolder} not found or not correct path");
                return false;
            }
            if (!Directory.Exists(options.LogsPath))
            {
                logger.Error($"Log folder {options.LogsPath} not found or not correct path");
                return false;
            }
            return true;
        }
    }
}
