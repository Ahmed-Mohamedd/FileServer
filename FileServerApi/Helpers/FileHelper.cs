using f= FileServer.Core.Entities;
using FileServer.Core.Repositories_Interfaces;
using FileServer.Core.Entities;

namespace FileServer.Api.Helpers
{
    public static class FileHelper
    {
        public static async Task<(string ,string)> UploadFile(IFormFile file, string FolderName)
        {
            /// get located folder path 
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName);

            /// get fileName and make its name unique[use guid]
            var FileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // get file path.
            var FilePath = Path.Combine(folderPath, FileName);

            //save file as streams[data per time]

            using var fs = new FileStream(FilePath, FileMode.Create);
            await file.CopyToAsync(fs);

            return (FileName,FilePath);

        }

        public static async Task CreateNewVersionForFile(IGenericRepository<FileVersion> _VersionsRepo, IGenericRepository<StorageQuota> _QuotaRepo, f.File ExisitingFile , string path,string userId)
        {
            // File exists, create a new version
            int newVersionNumber = ExisitingFile.FileVersions.Count + 1;

            var newVersion = new FileVersion
            {
                FileId = ExisitingFile.FileId,
                VersionNumber = newVersionNumber,
                FilePath = path + ".enc",
                UploadedBy = userId
            };

            await _VersionsRepo.Add(newVersion);
            var UserQuota = await _QuotaRepo.GetById(userId);
            UserQuota.UsedStorage+= ExisitingFile.FileSize;
        }

        public static async Task CreateNewFile(IGenericRepository<f.File> _FileRepo, IGenericRepository<FileVersion> _VersionsRepo, IGenericRepository<StorageQuota> _QuotaRepo, string FilePath,string FileName, string userId, IFormFile file , byte[] key , byte[] iv)
        {
            // New file upload
            var newFile = new f.File
            {
                FileName = FileName,
                MimeType = file.ContentType,
                FileSize = file.Length,
                FilePath = FilePath + ".enc",
                UploadedBy = userId,
                EncryptionKey = key,
                IV =iv
            };

            await _FileRepo.Add(newFile);

            // Add the initial version for the new file
            var initialVersion = new FileVersion
            {
                File = newFile,
                VersionNumber = 1,
                FilePath = FilePath + ".enc",
                UploadedBy = userId
            };

            await _VersionsRepo.Add(initialVersion);
            var UserQuota = await _QuotaRepo.GetById(userId);
            UserQuota.UsedStorage+= file.Length;
        }

    }
}
