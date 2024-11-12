using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Entities
{
    public  class StorageQuota
    {

        public long MaxStorage { get; set; } 
        public long? UsedStorage { get; set; }  // Current storage used

        [Key, ForeignKey(nameof(User))]  // Primary key and foreign key combined
        public string UserId { get; set; }  // FK to Users table (ApplicationUser)
        // Navigation property to the user
        public AppUser User { get; set; }
    }
}
