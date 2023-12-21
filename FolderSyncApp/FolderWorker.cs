using System.Security.Cryptography;
using NLog;
namespace FolderSyncApp
{
    public class FolderWorker
    {
        private string _target;
        private string _source;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public FolderWorker(string source, string target)
        {
            _target = target;
            _source = source;
        }

        public void StartScanning()
        {
            var sourceFolders = ScanFolder(_source);
            var targetFolders = ScanFolder(_target);
            foreach (var item in sourceFolders)
            {
                var file = targetFolders.Select(x => x).FirstOrDefault(y => y.Contains(item));
                if (string.IsNullOrEmpty(file))
                {
                    logger.Info($"File {Path.Combine(_target, item)} not found making backup");
                    CopyFile(item);
                }
                else
                {
                    var fileState = FilesAreSame(Path.Combine(_source, item), Path.Combine(_target, item));
                    if(fileState)
                    {
                        continue;
                    }
                    else
                    {
                        logger.Info($"File {Path.Combine(_target, item)} is changed updating backup");
                        CopyFile(item);
                    }
                }
            }
        }

        private bool FilesAreSame(string sourceFile, string targetFile)
        {
            var sourceHash = GetMD5Hash(sourceFile);
            var targetHash = GetMD5Hash(targetFile);
            if(sourceHash == null)
            {
                logger.Error($"Cant read source file {sourceFile}");
                return true;
            }
            if(targetHash == null)
            {
                logger.Error($"Cant read target file {targetFile}");
                return true;
            }
            for (var i = 0; i < sourceHash.Length; i++)
            {
                if (sourceHash[i] != targetHash[i])
                {
                    return false;
                }
            }
            return true;
        }
        private byte[]? GetMD5Hash(string file)
        {
            var myfile = new FileInfo(file);
            try
            {
                
                using var md5Creator = MD5.Create();
                {
                var stream = myfile.OpenRead();
                var fileHash = md5Creator.ComputeHash(stream);
                stream.Close();
                return fileHash;
                }
            }
            catch (Exception e)
            {
                logger.Error($"Problem with file: {e.Message}");
                return null;
            }
        }

        private void CopyFile(string partPath)
        {
            var path = Path.GetDirectoryName(partPath);
            var file = Path.GetFileName(partPath);
            string targetPath = Path.Combine(_target, path);
            DirectoryInfo dirInfo = new DirectoryInfo(targetPath);
            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(targetPath);
            }
            File.Copy(Path.Combine(_source, partPath), Path.Combine(targetPath, file), true);
        }

        private List<string> ScanFolder(string folder)
        {
            logger.Info($"Scanning folder {folder}");
            var list = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).ToList();
            return list.Select(x => x.Replace(folder, "").TrimStart('\\')).ToList();
        }
    }
}