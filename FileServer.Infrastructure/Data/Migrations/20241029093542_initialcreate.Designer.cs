﻿// <auto-generated />
using System;
using FileServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FileServer.Infrastructure.Data.Migrations
{
    [DbContext(typeof(FileContext))]
    [Migration("20241029093542_initialcreate")]
    partial class initialcreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("FileServer.Core.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FileServer.Core.Entities.File", b =>
                {
                    b.Property<Guid>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("UploadedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("FileId");

                    b.HasIndex("FileName")
                        .HasDatabaseName("IX_File_FileName");

                    b.HasIndex("UploadedBy")
                        .HasDatabaseName("IX_File_UploadedBy");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("FileServer.Core.Entities.File", b =>
                {
                    b.HasOne("FileServer.Core.Entities.AppUser", "User")
                        .WithMany("Files")
                        .HasForeignKey("UploadedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileServer.Core.Entities.AppUser", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
