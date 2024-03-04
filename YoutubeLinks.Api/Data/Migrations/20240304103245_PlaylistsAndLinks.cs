using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlaylistsAndLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Downloaded = table.Column<bool>(type: "bit", nullable: false),
                    PlaylistId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Links_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 4, 10, 32, 45, 583, DateTimeKind.Utc).AddTicks(8846), new DateTime(2024, 3, 4, 10, 32, 45, 583, DateTimeKind.Utc).AddTicks(8846) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 4, 10, 32, 45, 583, DateTimeKind.Utc).AddTicks(8846), new DateTime(2024, 3, 4, 10, 32, 45, 583, DateTimeKind.Utc).AddTicks(8846) });

            migrationBuilder.CreateIndex(
                name: "IX_Links_PlaylistId",
                table: "Links",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885), new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885), new DateTime(2024, 3, 3, 16, 39, 48, 257, DateTimeKind.Utc).AddTicks(6885) });
        }
    }
}
