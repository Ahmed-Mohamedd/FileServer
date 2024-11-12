using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Entities
{
    public class FileVersion
    {
        [Key]
        public Guid VersionId { get; set; }  // Primary Key

        public int VersionNumber { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } // Path to the file version
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public Guid FileId { get; set; }     // Foreign Key to Files table
        public File File { get; set; } // Navigation property


        public string UploadedBy { get; set; }  // Foreign Key to Users table (UserId)
        public AppUser User { get; set; } // Navigation property
    }
}
