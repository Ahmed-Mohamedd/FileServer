using f = FileServer.Core.Entities ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Infrastructure.Data.Configurations
{
    public class FileConfigurations : IEntityTypeConfiguration<f.File>
    {
        public void Configure(EntityTypeBuilder<f.File> builder)
        {
            builder.HasKey(f => f.FileId);

            builder.HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UploadedBy)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(f => f.UploadedBy)
           .HasDatabaseName("IX_File_UploadedBy");

            builder.HasIndex(f => f.FileName)
           .HasDatabaseName("IX_File_FileName");




        }
    }
}
