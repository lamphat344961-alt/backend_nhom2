using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_nhom2.Migrations
{
    /// <inheritdoc />
    public partial class capnhat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "XE",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "XE",
                keyColumn: "BS_XE",
                keyValue: "51A-00001",
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "XE",
                keyColumn: "BS_XE",
                keyValue: "51B-00002",
                column: "UserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_XE_UserId",
                table: "XE",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_XE_Users_UserId",
                table: "XE",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XE_Users_UserId",
                table: "XE");

            migrationBuilder.DropIndex(
                name: "IX_XE_UserId",
                table: "XE");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "XE");
        }
    }
}
