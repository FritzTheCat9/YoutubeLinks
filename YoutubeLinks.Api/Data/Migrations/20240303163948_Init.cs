using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    EmailConfirmationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "EmailConfirmationToken", "EmailConfirmed", "IsAdmin", "Modified", "Password", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885), "freakfightsfan@gmail.com", null, true, true, new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885), "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==", "Admin" },
                    { 2, new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885), "freakfightsfan1@gmail.com", null, true, false, new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885), "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
