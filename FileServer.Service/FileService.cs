using FileServer.Core.Repositories_Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Service
{
    public class FileService : IFileService
    {

        public async Task<(string, string)> UploadFile(IFormFile file, string FolderName, byte[] key, byte[] iv)
        {
            /// get located folder path 
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName);

            /// get fileName and make its name unique[use guid]
            var FileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // get file path.
            var compressedEncryptedPath = Path.Combine(folderPath, FileName);

            //save file as streams[data per time]
            using (var originalStream = new MemoryStream())
            {
                await file.CopyToAsync(originalStream);
                originalStream.Position = 0;                                                      // Reset stream position
               await SaveCompressedFile(originalStream , compressedEncryptedPath , key , iv);    // Step 1: Compress the file
            }

            return (FileName, compressedEncryptedPath);
        }

        public async Task SaveCompressedFile(Stream inputStream, string compressedEncryptedPath, byte[] key, byte[] iv)
        {
            using (var compressedStream = new MemoryStream())
            using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
            {
                await inputStream.CopyToAsync(gzipStream);
                compressedStream.Position = 0;                                               // Reset compressed stream position
                await EncryptFileAsync(compressedStream, compressedEncryptedPath, key, iv); // Step 2: Encrypt the compressed data
            }
        }

        public  async Task EncryptFileAsync(Stream inputStream, string outputPath, byte[] key, byte[] iv)
        {
            using (var fileStream = new FileStream(outputPath + ".enc", FileMode.Create))
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    await inputStream.CopyToAsync(cryptoStream);
                }
            }
        }


        public  (byte[] Key, byte[] IV) GenerateEncryptionKey()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                return (aes.Key, aes.IV);
            }
        }
        public  async Task DecryptFileAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    await cryptoStream.CopyToAsync(outputStream);
                }
            }
        }

        public async Task DecompressFileAsync(Stream compressedStream, Stream outputStream)  // Decompress the decrypted file stream
        {
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(outputStream);
            }
        }
    }
}
