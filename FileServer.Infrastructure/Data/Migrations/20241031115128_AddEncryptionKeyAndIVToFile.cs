using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileServer.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptionKeyAndIVToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "EncryptionKey",
                table: "Files",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "IV",
                table: "Files",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptionKey",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IV",
                table: "Files");
        }
    }
}
