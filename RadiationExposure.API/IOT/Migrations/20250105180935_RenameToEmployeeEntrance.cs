using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IOT.Migrations
{
    /// <inheritdoc />
    public partial class RenameToEmployeeEntrance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesRadiations");

            migrationBuilder.CreateTable(
                name: "EmployeeEntrance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    EntranceTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEntrance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeEntrance_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeEntrance_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEntrance_EmployeeId",
                table: "EmployeeEntrance",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEntrance_ZoneId",
                table: "EmployeeEntrance",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeEntrance");

            migrationBuilder.CreateTable(
                name: "EmployeesRadiations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    EntranceTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesRadiations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeesRadiations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesRadiations_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesRadiations_EmployeeId",
                table: "EmployeesRadiations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesRadiations_ZoneId",
                table: "EmployeesRadiations",
                column: "ZoneId");
        }
    }
}
