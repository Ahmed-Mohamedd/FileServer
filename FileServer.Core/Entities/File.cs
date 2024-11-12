using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Entities
{
    public class File
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now ;
        public byte[] EncryptionKey { get; set; } // Stores the encryption key
        public byte[] IV { get; set; }            // Stores the initialization vector
        public bool IsDeleted { get; set; } = false;
 
        public string UploadedBy { get; set; }
        // Navigation property to the user
        public AppUser User { get; set; }

        public IList<FileVersion> FileVersions { get; set; } = new List<FileVersion>();
    }
}
