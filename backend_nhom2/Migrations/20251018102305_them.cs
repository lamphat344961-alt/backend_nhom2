using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_nhom2.Migrations
{
    /// <inheritdoc />
    public partial class them : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DONHANG_LOAIHANG_MALOAI",
                table: "DONHANG");

            migrationBuilder.DropIndex(
                name: "IX_DONHANG_MALOAI",
                table: "DONHANG");

            migrationBuilder.AlterColumn<string>(
                name: "MALOAI",
                table: "DONHANG",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiHangMALOAI",
                table: "DONHANG",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DONHANG",
                keyColumn: "MADON",
                keyValue: "DH001",
                column: "LoaiHangMALOAI",
                value: null);

            migrationBuilder.UpdateData(
                table: "DONHANG",
                keyColumn: "MADON",
                keyValue: "DH002",
                column: "LoaiHangMALOAI",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_LoaiHangMALOAI",
                table: "DONHANG",
                column: "LoaiHangMALOAI");

            migrationBuilder.AddForeignKey(
                name: "FK_DONHANG_LOAIHANG_LoaiHangMALOAI",
                table: "DONHANG",
                column: "LoaiHangMALOAI",
                principalTable: "LOAIHANG",
                principalColumn: "MALOAI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DONHANG_LOAIHANG_LoaiHangMALOAI",
                table: "DONHANG");

            migrationBuilder.DropIndex(
                name: "IX_DONHANG_LoaiHangMALOAI",
                table: "DONHANG");

            migrationBuilder.DropColumn(
                name: "LoaiHangMALOAI",
                table: "DONHANG");

            migrationBuilder.AlterColumn<string>(
                name: "MALOAI",
                table: "DONHANG",
                type: "nvarchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_MALOAI",
                table: "DONHANG",
                column: "MALOAI");

            migrationBuilder.AddForeignKey(
                name: "FK_DONHANG_LOAIHANG_MALOAI",
                table: "DONHANG",
                column: "MALOAI",
                principalTable: "LOAIHANG",
                principalColumn: "MALOAI",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
