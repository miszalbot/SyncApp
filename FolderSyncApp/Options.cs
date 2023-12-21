using CommandLine;

namespace FolderSyncApp
{
    public class Options
    {
        [Option('s', "SourceFolder")]
        public required string SourceFolder { get; set; }

        [Option('t', "TargetFolder")]
        public required string TargetFolder { get; set; }

        [Option('l', "LogsPath")]
        public string LogsPath { get; set; }

        [Option('i', "Interval")]
        public required int SyncInterval { get; set; }




    }
}