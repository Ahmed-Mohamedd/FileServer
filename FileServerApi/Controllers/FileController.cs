using f=  FileServer.Core.Entities;
using FileServer.Core.Repositories_Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FileServerApi.Errors;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using FileServer.Core.Specifications;
using FileServer.Core.Entities;
using FileServer.Api.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FileServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".png", ".pdf", ".docx" };
        private readonly long _maxFileSize = 50 * 1024 * 1024; // 50 MB
        private readonly IGenericRepository<f.File> _FileRepo;
        private readonly IGenericRepository<FileVersion> _VersionsRepo;
        private readonly IGenericRepository<StorageQuota> _QuotaRepo;
        private readonly IFileService _FileService;
        public FileController(IGenericRepository<f.File> fileRepo, IGenericRepository<FileVersion> versionsRepo, IFileService fileService, IGenericRepository<StorageQuota> quotaRepo)
        {
            _FileRepo=fileRepo;
            _VersionsRepo=versionsRepo;
            _FileService=fileService;
            _QuotaRepo=quotaRepo;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] List<IFormFile> files)
        {

            if (files == null || !files.Any())
                return BadRequest("No files uploaded.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var spec = new FileWithVersionsSpec(new FileParams());
            var Allfiles = await _FileRepo.GetAllWithSpec(spec);
            List<object> uploadResults = new();

            foreach (var file in files)
            {

                // Validate file size
                if (file.Length > _maxFileSize)
                {
                    uploadResults.Add(new { FileName = file.FileName, Error = "File size exceeds limit." });
                    continue;
                }

                // Validate file extension
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    uploadResults.Add(new { FileName = file.FileName, Error = "Invalid file type." });
                    continue;
                }

                // Generate encryption key and IV
                var (key, iv) = _FileService.GenerateEncryptionKey();

                var UploadedFileResult = await _FileService.UploadFile(file, "Uploads", key, iv); // upload file to destination
                var ExisitingFile = Allfiles.FirstOrDefault(f => f.FileName.Split("_")[1] == file.FileName); // Check if the file already exists

                if (ExisitingFile != null)
                {
                    // File exists, create a new version
                    await FileHelper.CreateNewVersionForFile(_VersionsRepo, _QuotaRepo, ExisitingFile, UploadedFileResult.Item2, userId);
                }
                else
                {
                    // New file upload
                    await FileHelper.CreateNewFile(_FileRepo, _VersionsRepo, _QuotaRepo, UploadedFileResult.Item2, UploadedFileResult.Item1, userId, file, key, iv);
                }
                uploadResults.Add(new { FileName = file.FileName, Message = "File uploaded successfully." });
            }

            await _FileRepo.SaveChangesAsync();  // Save changes to the database
            return Ok(uploadResults);

        }


        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var spec = new FileWithVersionsSpec(fileId);

            // Fetch file metadata from the database
            var fileRecord = await _FileRepo.GetByIdWithSpec(spec);
            if (fileRecord == null)
                return NotFound("File not found.");

            // Set up streams for decryption and decompression
            var encryptedFilePath = fileRecord.FilePath;
            var decryptedStream = new MemoryStream();
            var decompressedStream = new MemoryStream();

            try
            {
                // Step 1: Open the encrypted file
                using (var encryptedStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Step 2: Decrypt the file
                    await _FileService.DecryptFileAsync(encryptedStream, decryptedStream, fileRecord.EncryptionKey, fileRecord.IV);
                    decryptedStream.Position = 0; // Reset stream position for decompression

                    // Step 3: Decompress the file
                    await _FileService.DecompressFileAsync(decryptedStream, decompressedStream);
                    decompressedStream.Position = 0; // Reset stream position for download
                }

                // Step 4: Return the decompressed file as a downloadable response
                return File(decompressedStream, fileRecord.MimeType, fileRecord.FileName);
            }
            catch (Exception ex)
            {
                // Handle exceptions if needed (e.g., log the error)
                return StatusCode(500, "Error processing file download.");
            }
        }

        [HttpDelete("delete/{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileId) // this will delete the main file with its versions too.
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var spec = new FileWithVersionsSpec(fileId);

            // Find the file record in the database
            var fileRecord = await _FileRepo.GetByIdWithSpec(spec);
            if (fileRecord == null)
                return NotFound("File not found.");

            try
            {
                // Delete the main file
                if (System.IO.File.Exists(fileRecord.FilePath))
                    System.IO.File.Delete(fileRecord.FilePath);

                // Remove the file record from the database
                await _FileRepo.Delete(spec);
                await _FileRepo.SaveChangesAsync();

                return Ok(new { Message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                // Handle and log exceptions if necessary
                return StatusCode(500, "An error occurred while deleting the file.");
            }
        }


        [HttpDelete("delete/{fileId}/version/{versionNumber}")]
        public async Task<IActionResult> DeleteFileVersion(string fileId, int versionNumber)
        {
            
            // Find the file version record in the database
            var fileVersions = await _VersionsRepo.GetAll();
            var fileVersion = fileVersions.FirstOrDefault(fv => (fv.FileId == new Guid(fileId)) && (fv.VersionNumber == versionNumber));

            if (fileVersion == null)
                return NotFound("File version not found.");

            try
            {
                // Delete the file version from storage
                if (System.IO.File.Exists(fileVersion.FilePath))
                    System.IO.File.Delete(fileVersion.FilePath);

                // Remove the file version record from the database
                _VersionsRepo.DeleteWithoutSpec(fileVersion);
                await _VersionsRepo.SaveChangesAsync();

                return Ok(new { Message = "File version deleted successfully." });
            }
            catch (Exception ex)
            {
                // Handle and log exceptions if necessary
                return StatusCode(500, "An error occurred while deleting the file version.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<f.File>>> GetFiles([FromQuery]FileParams fileParams)
        {
            var spec = new FileWithVersionsSpec(fileParams);
            var files = await _FileRepo.GetAll();
            return Ok(  new Pagination<f.File>(fileParams.PageIndex,fileParams.PageSize, files.Count(),   await _FileRepo.GetAllWithSpec(spec)));
        }

    }
}
