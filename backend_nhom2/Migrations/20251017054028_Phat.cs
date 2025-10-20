using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend_nhom2.Migrations
{
    /// <inheritdoc />
    public partial class Phat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DIEMGIAO",
                columns: table => new
                {
                    D_DD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VITRI = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TEN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIEMGIAO", x => x.D_DD);
                });

            migrationBuilder.CreateTable(
                name: "LOAIHANG",
                columns: table => new
                {
                    MALOAI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TENLOAI = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SL = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAIHANG", x => x.MALOAI);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "XE",
                columns: table => new
                {
                    BS_XE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TT_XE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TENXE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XE", x => x.BS_XE);
                });

            migrationBuilder.CreateTable(
                name: "DONHANG",
                columns: table => new
                {
                    MADON = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MALOAI = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    NGAYLAP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TONGTIEN = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DONHANG", x => x.MADON);
                    table.ForeignKey(
                        name: "FK_DONHANG_LOAIHANG_MALOAI",
                        column: x => x.MALOAI,
                        principalTable: "LOAIHANG",
                        principalColumn: "MALOAI",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HANGHOA",
                columns: table => new
                {
                    MAHH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TENHH = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SL = table.Column<int>(type: "int", nullable: false),
                    MALOAI = table.Column<string>(type: "nvarchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HANGHOA", x => x.MAHH);
                    table.ForeignKey(
                        name: "FK_HANGHOA_LOAIHANG_MALOAI",
                        column: x => x.MALOAI,
                        principalTable: "LOAIHANG",
                        principalColumn: "MALOAI",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CT_DIEMGIAO",
                columns: table => new
                {
                    D_DD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BS_XE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MADON = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NGAYGIAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TRANGTHAI = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DIEMGIAO", x => new { x.D_DD, x.BS_XE, x.MADON });
                    table.ForeignKey(
                        name: "FK_CT_DIEMGIAO_DIEMGIAO_D_DD",
                        column: x => x.D_DD,
                        principalTable: "DIEMGIAO",
                        principalColumn: "D_DD",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CT_DIEMGIAO_DONHANG_MADON",
                        column: x => x.MADON,
                        principalTable: "DONHANG",
                        principalColumn: "MADON",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CT_DIEMGIAO_XE_BS_XE",
                        column: x => x.BS_XE,
                        principalTable: "XE",
                        principalColumn: "BS_XE",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CT_DONHANG",
                columns: table => new
                {
                    MAHH = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    MADON = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DONGIA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SL = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DONHANG", x => new { x.MAHH, x.MADON });
                    table.ForeignKey(
                        name: "FK_CT_DONHANG_DONHANG_MADON",
                        column: x => x.MADON,
                        principalTable: "DONHANG",
                        principalColumn: "MADON",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CT_DONHANG_HANGHOA_MAHH",
                        column: x => x.MAHH,
                        principalTable: "HANGHOA",
                        principalColumn: "MAHH",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DIEMGIAO",
                columns: new[] { "D_DD", "TEN", "VITRI" },
                values: new object[,]
                {
                    { "DG01", "Kho Quận 1", "Q1" },
                    { "DG02", "Kho Quận 7", "Q7" }
                });

            migrationBuilder.InsertData(
                table: "LOAIHANG",
                columns: new[] { "MALOAI", "SL", "TENLOAI" },
                values: new object[,]
                {
                    { "L1", 0, "Điện tử" },
                    { "L2", 0, "Thời trang" }
                });

            migrationBuilder.InsertData(
                table: "XE",
                columns: new[] { "BS_XE", "TENXE", "TT_XE" },
                values: new object[,]
                {
                    { "51A-00001", "Xe Tải Nhỏ", "Sẵn sàng" },
                    { "51B-00002", "Xe Bán Tải", "Bảo dưỡng" }
                });

            migrationBuilder.InsertData(
                table: "DONHANG",
                columns: new[] { "MADON", "MALOAI", "NGAYLAP", "TONGTIEN" },
                values: new object[,]
                {
                    { "DH001", "L1", new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { "DH002", "L2", new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m }
                });

            migrationBuilder.InsertData(
                table: "HANGHOA",
                columns: new[] { "MAHH", "MALOAI", "SL", "TENHH" },
                values: new object[,]
                {
                    { "H001", "L1", 100, "Tai nghe" },
                    { "H002", "L2", 200, "Áo thun" }
                });

            migrationBuilder.InsertData(
                table: "CT_DIEMGIAO",
                columns: new[] { "BS_XE", "D_DD", "MADON", "NGAYGIAO", "TRANGTHAI" },
                values: new object[,]
                {
                    { "51A-00001", "DG01", "DH001", null, "CHO_GIAO" },
                    { "51B-00002", "DG02", "DH002", null, "CHO_GIAO" }
                });

            migrationBuilder.InsertData(
                table: "CT_DONHANG",
                columns: new[] { "MADON", "MAHH", "DONGIA", "SL" },
                values: new object[,]
                {
                    { "DH001", "H001", 350000m, 2 },
                    { "DH001", "H002", 120000m, 3 },
                    { "DH002", "H002", 110000m, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CT_DIEMGIAO_BS_XE",
                table: "CT_DIEMGIAO",
                column: "BS_XE");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DIEMGIAO_MADON",
                table: "CT_DIEMGIAO",
                column: "MADON");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DONHANG_MADON",
                table: "CT_DONHANG",
                column: "MADON");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_MALOAI",
                table: "DONHANG",
                column: "MALOAI");

            migrationBuilder.CreateIndex(
                name: "IX_HANGHOA_MALOAI",
                table: "HANGHOA",
                column: "MALOAI");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CT_DIEMGIAO");

            migrationBuilder.DropTable(
                name: "CT_DONHANG");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DIEMGIAO");

            migrationBuilder.DropTable(
                name: "XE");

            migrationBuilder.DropTable(
                name: "DONHANG");

            migrationBuilder.DropTable(
                name: "HANGHOA");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "LOAIHANG");
        }
    }
}
