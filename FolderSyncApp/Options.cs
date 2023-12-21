using CommandLine;
using NLog;

namespace FolderSyncApp
{
    public class Options
    {

        [Option('s', "SourceFolder", Required = true, HelpText = "Path to source folder")]
        public required string SourceFolder { get; set; }

        [Option('t', "TargetFolder", Required = true, HelpText = "Path to target folder")]
        public required string TargetFolder { get; set; }

        [Option('l', "LogsPath", Required = true, HelpText = "Path to save logs")]
        public required string LogsPath { get; set; }

        [Option('i', "Interval", Required = true, HelpText = "Time interval in seconds to check folder")]
        public required int SyncInterval { get; set; }

    }
}