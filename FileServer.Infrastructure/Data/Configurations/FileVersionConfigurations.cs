using FileServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Infrastructure.Data.Configurations
{
    public class FileVersionConfigurations : IEntityTypeConfiguration<FileVersion>
    {
        public void Configure(EntityTypeBuilder<FileVersion> builder)
        {

            // Set the primary key
            builder.HasKey(v => v.VersionId);

            // Configure properties
            builder.Property(v => v.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.VersionNumber)
           .IsRequired();

            builder.Property(v => v.UploadDate)
                .HasDefaultValueSql("GETDATE()");

            // Configure the relationship with ApplicationUser (Many-to-One)
            builder.HasOne(v => v.User)
                .WithMany()  // No need for reverse navigation if not required
                .HasForeignKey(v => v.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent user deletion if versions exist

            // Configure the relationship with FileEntity (Many-to-One)
            builder.HasOne(v => v.File)
                .WithMany(f => f.FileVersions)  // One File can have many versions
                .HasForeignKey(v => v.FileId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete versions if file is deleted


        }
    }
}
