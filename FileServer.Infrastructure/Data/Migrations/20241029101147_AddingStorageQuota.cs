using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileServer.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingStorageQuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageQuotas",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    MaxStorage = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 10737418240L),
                    UsedStorage = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageQuotas", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_StorageQuotas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageQuotas");
        }
    }
}
