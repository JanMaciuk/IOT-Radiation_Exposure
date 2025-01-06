using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOT.Migrations
{
    /// <inheritdoc />
    public partial class SnakeCaseNamingConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeEntrance_Employees_EmployeeId",
                table: "EmployeeEntrance");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeEntrance_Zones_ZoneId",
                table: "EmployeeEntrance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Zones",
                table: "Zones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeEntrance",
                table: "EmployeeEntrance");

            migrationBuilder.RenameTable(
                name: "Zones",
                newName: "zones");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "EmployeeEntrance",
                newName: "employee_entrance");

            migrationBuilder.RenameColumn(
                name: "Radiation",
                table: "zones",
                newName: "radiation");

            migrationBuilder.RenameColumn(
                name: "Info",
                table: "zones",
                newName: "info");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "zones",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "employees",
                newName: "surname");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "employees",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Card",
                table: "employees",
                newName: "card");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "employees",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "employee_entrance",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ZoneId",
                table: "employee_entrance",
                newName: "zone_id");

            migrationBuilder.RenameColumn(
                name: "ExitTime",
                table: "employee_entrance",
                newName: "exit_time");

            migrationBuilder.RenameColumn(
                name: "EntranceTime",
                table: "employee_entrance",
                newName: "entrance_time");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "employee_entrance",
                newName: "employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeEntrance_ZoneId",
                table: "employee_entrance",
                newName: "ix_employee_entrance_zone_id");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeEntrance_EmployeeId",
                table: "employee_entrance",
                newName: "ix_employee_entrance_employee_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_zones",
                table: "zones",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_employees",
                table: "employees",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_employee_entrance",
                table: "employee_entrance",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_employee_entrance_employees_employee_id",
                table: "employee_entrance",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_entrance_zones_zone_id",
                table: "employee_entrance",
                column: "zone_id",
                principalTable: "zones",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employee_entrance_employees_employee_id",
                table: "employee_entrance");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_entrance_zones_zone_id",
                table: "employee_entrance");

            migrationBuilder.DropPrimaryKey(
                name: "pk_zones",
                table: "zones");

            migrationBuilder.DropPrimaryKey(
                name: "pk_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "pk_employee_entrance",
                table: "employee_entrance");

            migrationBuilder.RenameTable(
                name: "zones",
                newName: "Zones");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "employee_entrance",
                newName: "EmployeeEntrance");

            migrationBuilder.RenameColumn(
                name: "radiation",
                table: "Zones",
                newName: "Radiation");

            migrationBuilder.RenameColumn(
                name: "info",
                table: "Zones",
                newName: "Info");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Zones",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "surname",
                table: "Employees",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Employees",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "card",
                table: "Employees",
                newName: "Card");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EmployeeEntrance",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "zone_id",
                table: "EmployeeEntrance",
                newName: "ZoneId");

            migrationBuilder.RenameColumn(
                name: "exit_time",
                table: "EmployeeEntrance",
                newName: "ExitTime");

            migrationBuilder.RenameColumn(
                name: "entrance_time",
                table: "EmployeeEntrance",
                newName: "EntranceTime");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "EmployeeEntrance",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "ix_employee_entrance_zone_id",
                table: "EmployeeEntrance",
                newName: "IX_EmployeeEntrance_ZoneId");

            migrationBuilder.RenameIndex(
                name: "ix_employee_entrance_employee_id",
                table: "EmployeeEntrance",
                newName: "IX_EmployeeEntrance_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Zones",
                table: "Zones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeEntrance",
                table: "EmployeeEntrance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeEntrance_Employees_EmployeeId",
                table: "EmployeeEntrance",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeEntrance_Zones_ZoneId",
                table: "EmployeeEntrance",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
