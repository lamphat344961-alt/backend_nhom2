using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_nhom2.Migrations
{
    /// <inheritdoc />
    public partial class suaquanhe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CT_DIEMGIAO_XE_BS_XE",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CT_DIEMGIAO",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropIndex(
                name: "IX_CT_DIEMGIAO_BS_XE",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropIndex(
                name: "IX_CT_DIEMGIAO_D_DD_MADON_BS_XE",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropIndex(
                name: "IX_CT_DIEMGIAO_MADON",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropColumn(
                name: "BS_XE",
                table: "CT_DIEMGIAO");

            migrationBuilder.AlterColumn<string>(
                name: "MALOAI",
                table: "DONHANG",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BS_XE",
                table: "DONHANG",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VITRI",
                table: "DIEMGIAO",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TEN",
                table: "DIEMGIAO",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CT_DIEMGIAO",
                table: "CT_DIEMGIAO",
                columns: new[] { "MADON", "D_DD" });

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_BS_XE",
                table: "DONHANG",
                column: "BS_XE");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DIEMGIAO_D_DD",
                table: "CT_DIEMGIAO",
                column: "D_DD");

            migrationBuilder.AddForeignKey(
                name: "FK_DONHANG_XE_BS_XE",
                table: "DONHANG",
                column: "BS_XE",
                principalTable: "XE",
                principalColumn: "BS_XE",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DONHANG_XE_BS_XE",
                table: "DONHANG");

            migrationBuilder.DropIndex(
                name: "IX_DONHANG_BS_XE",
                table: "DONHANG");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CT_DIEMGIAO",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropIndex(
                name: "IX_CT_DIEMGIAO_D_DD",
                table: "CT_DIEMGIAO");

            migrationBuilder.DropColumn(
                name: "BS_XE",
                table: "DONHANG");

            migrationBuilder.AlterColumn<string>(
                name: "MALOAI",
                table: "DONHANG",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VITRI",
                table: "DIEMGIAO",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TEN",
                table: "DIEMGIAO",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BS_XE",
                table: "CT_DIEMGIAO",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CT_DIEMGIAO",
                table: "CT_DIEMGIAO",
                columns: new[] { "D_DD", "MADON" });

            migrationBuilder.CreateIndex(
                name: "IX_CT_DIEMGIAO_BS_XE",
                table: "CT_DIEMGIAO",
                column: "BS_XE");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DIEMGIAO_D_DD_MADON_BS_XE",
                table: "CT_DIEMGIAO",
                columns: new[] { "D_DD", "MADON", "BS_XE" });

            migrationBuilder.CreateIndex(
                name: "IX_CT_DIEMGIAO_MADON",
                table: "CT_DIEMGIAO",
                column: "MADON");

            migrationBuilder.AddForeignKey(
                name: "FK_CT_DIEMGIAO_XE_BS_XE",
                table: "CT_DIEMGIAO",
                column: "BS_XE",
                principalTable: "XE",
                principalColumn: "BS_XE",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
