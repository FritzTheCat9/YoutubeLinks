using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkVideoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoId",
                table: "Links",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924), new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924), new DateTime(2024, 3, 9, 16, 48, 6, 921, DateTimeKind.Utc).AddTicks(1924) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Links");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 4, 20, 13, 9, 633, DateTimeKind.Utc).AddTicks(2940), new DateTime(2024, 3, 4, 20, 13, 9, 633, DateTimeKind.Utc).AddTicks(2940) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 4, 20, 13, 9, 633, DateTimeKind.Utc).AddTicks(2940), new DateTime(2024, 3, 4, 20, 13, 9, 633, DateTimeKind.Utc).AddTicks(2940) });
        }
    }
}
