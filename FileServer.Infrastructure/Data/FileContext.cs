using FileServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using f = FileServer.Core.Entities;

namespace FileServer.Infrastructure.Data
{
    public class FileContext:DbContext
    {
        public FileContext(DbContextOptions<FileContext> options):base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<f.File> Files { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DbSet<StorageQuota> StorageQuotas { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}
