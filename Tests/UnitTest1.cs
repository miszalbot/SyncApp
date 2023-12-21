using FolderSyncApp;

namespace Tests
{

    public class FunctionTests
    {
        [Test]
        public void ScanFolders()
        {
            var _source = "C:\\TestFolders\\Source";
            var _target = "C:\\TestFolders\\Target";

            var worker = new FolderWorker(_source, _target);
            worker.StartScanning();
        }
    }
}