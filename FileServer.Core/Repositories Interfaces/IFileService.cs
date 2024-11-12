using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Repositories_Interfaces
{
    public interface IFileService
    {
        Task<(string, string)> UploadFile(IFormFile file, string FolderName, byte[] key, byte[] iv);
        Task SaveCompressedFile(Stream inputStream, string compressedEncryptedPath, byte[] key, byte[] iv);
        (byte[] Key, byte[] IV) GenerateEncryptionKey();
        Task EncryptFileAsync(Stream inputStream, string outputPath, byte[] key, byte[] iv);
        Task DecryptFileAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv);
        Task DecompressFileAsync(Stream compressedStream, Stream outputStream);
    }
}
