using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Entities
{
    public class AppUser
    {
        public string Id { get; set; }
        // Navigation property: A user can have many files
        public ICollection<File> Files { get; set; } = new List<File>();

        // One-to-One relationship with StorageQuota
        public StorageQuota StorageQuota { get; set; }
    }
}
