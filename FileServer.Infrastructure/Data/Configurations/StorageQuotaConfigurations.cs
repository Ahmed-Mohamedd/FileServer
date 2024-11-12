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
    public class StorageQuotaConfigurations : IEntityTypeConfiguration<StorageQuota>
    {
        public void Configure(EntityTypeBuilder<StorageQuota> builder)
        {
            // Set the primary key (which is also the foreign key to ApplicationUser)
            builder.HasKey(sq => sq.UserId);

            // Configure the one-to-one relationship with ApplicationUser
            builder.HasOne(sq => sq.User)
                .WithOne(u => u.StorageQuota)  // One user has one quota
                .HasForeignKey<StorageQuota>(sq => sq.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete the quota if the user is deleted

            builder.Property(sq => sq.MaxStorage)
            .IsRequired()
            .HasDefaultValue(10L * 1024 * 1024 * 1024);  // 10 GB in bytes

            // Configure the UsedStorage property (defaulting to 0)
            builder.Property(sq => sq.UsedStorage)
                .IsRequired()
                .HasDefaultValue(0L);  // 0 bytes used by default

        }
    }
}
