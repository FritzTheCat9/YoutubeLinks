using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeLinks.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class User_EmailConfirmationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForgotPasswordToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "ForgotPasswordToken", "Modified" },
                values: new object[] { new DateTime(2024, 5, 12, 19, 26, 48, 403, DateTimeKind.Utc).AddTicks(1639), null, new DateTime(2024, 5, 12, 19, 26, 48, 403, DateTimeKind.Utc).AddTicks(1639) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "ForgotPasswordToken", "Modified" },
                values: new object[] { new DateTime(2024, 5, 12, 19, 26, 48, 403, DateTimeKind.Utc).AddTicks(1639), null, new DateTime(2024, 5, 12, 19, 26, 48, 403, DateTimeKind.Utc).AddTicks(1639) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForgotPasswordToken",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956), new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956), new DateTime(2024, 5, 1, 15, 33, 23, 935, DateTimeKind.Utc).AddTicks(5956) });
        }
    }
}
