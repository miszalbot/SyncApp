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
            /*
            Main scanning code
            Get all files in both source and target folder
            Compare files for presence and hash
            If missing or different source file is copied to target folder
            */
            logger.Info($"Scanning Source and Target folders");
            var sourceFolders = ScanFolder(_source);
            var targetFolders = ScanFolder(_target);
            foreach (var item in sourceFolders)
            {
                var file = targetFolders.Select(x => x).FirstOrDefault(y => y.Contains(item));
                if (string.IsNullOrEmpty(file))
                {
                    logger.Info($"File {Path.Combine(_source, item)} not found making backup");
                    CopyFile(item);
                }
                else
                {
                    var fileState = FilesAreSame(Path.Combine(_source, item), Path.Combine(_target, item));
                    if(!fileState)
                    {
                        logger.Info($"File {Path.Combine(_source, item)} is changed updating backup");
                        CopyFile(item);
                    }
                }
            }

            // Check deleted items from source folder
            sourceFolders = ScanFolder(_source);
            targetFolders = ScanFolder(_target);
            if(sourceFolders.Count < targetFolders.Count)
            {
                var missingSource = targetFolders.Except(sourceFolders);
                foreach(var missing in missingSource)
                {                    
                    DeleteFile(missing);
                }
            }
        }



        private bool FilesAreSame(string sourceFile, string targetFile)
        {
            //Comparing two file using md5hash
            // If file is not possible to read it is skipped
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
            //Geting Hash of file if file openning fail log error
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
            //Checking if folder exist(if not is created) and copy file to target destination
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

        private void DeleteFile(string partPath)
        {
            // File delete if folder is empty is deleted also
            logger.Info($"File {Path.Combine(_target, partPath)} missing in source folder deleting from target");
            var dirPath = Path.Combine(_target,Path.GetDirectoryName(partPath));  
            var fullPath = Path.Combine(_target,partPath);          
            File.Delete(fullPath);
            if(Directory.GetFiles(dirPath).Count()== 0)
            {
                Directory.Delete(dirPath);
            }
        }

        private List<string> ScanFolder(string folder)
        {
            // ScanFolder for files and subfolders and remove source folder part of path            
            var list = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).ToList();
            return list.Select(x => x.Replace(folder, "").TrimStart('\\')).ToList();
        }
    }
}