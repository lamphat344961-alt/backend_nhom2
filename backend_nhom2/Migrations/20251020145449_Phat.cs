using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    TEN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: true)
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
                name: "RoutePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DONHANG",
                columns: table => new
                {
                    MADON = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MALOAI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiHangMALOAI = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    NGAYLAP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TONGTIEN = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DONHANG", x => x.MADON);
                    table.ForeignKey(
                        name: "FK_DONHANG_LOAIHANG_LoaiHangMALOAI",
                        column: x => x.LoaiHangMALOAI,
                        principalTable: "LOAIHANG",
                        principalColumn: "MALOAI");
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
                name: "RouteStops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoutePlanId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Lng = table.Column<double>(type: "float", nullable: false),
                    EtaEpoch = table.Column<long>(type: "bigint", nullable: false),
                    EtaIso = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Polyline = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStops_RoutePlans_RoutePlanId",
                        column: x => x.RoutePlanId,
                        principalTable: "RoutePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "XE",
                columns: table => new
                {
                    BS_XE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TT_XE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TENXE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XE", x => x.BS_XE);
                    table.ForeignKey(
                        name: "FK_XE_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "CT_DIEMGIAO",
                columns: table => new
                {
                    D_DD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MADON = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BS_XE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NGAYGIAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TRANGTHAI = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    WindowStart = table.Column<long>(type: "bigint", nullable: true),
                    WindowEnd = table.Column<long>(type: "bigint", nullable: true),
                    ServiceMinutes = table.Column<int>(type: "int", nullable: true, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DIEMGIAO", x => new { x.D_DD, x.MADON });
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
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CT_DONHANG_MADON",
                table: "CT_DONHANG",
                column: "MADON");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_LoaiHangMALOAI",
                table: "DONHANG",
                column: "LoaiHangMALOAI");

            migrationBuilder.CreateIndex(
                name: "IX_HANGHOA_MALOAI",
                table: "HANGHOA",
                column: "MALOAI");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RoutePlanId_Order",
                table: "RouteStops",
                columns: new[] { "RoutePlanId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_XE_UserId",
                table: "XE",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CT_DIEMGIAO");

            migrationBuilder.DropTable(
                name: "CT_DONHANG");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "DIEMGIAO");

            migrationBuilder.DropTable(
                name: "XE");

            migrationBuilder.DropTable(
                name: "DONHANG");

            migrationBuilder.DropTable(
                name: "HANGHOA");

            migrationBuilder.DropTable(
                name: "RoutePlans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "LOAIHANG");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
