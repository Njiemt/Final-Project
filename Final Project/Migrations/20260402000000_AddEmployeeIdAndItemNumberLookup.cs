using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations;

[DbContext(typeof(WarehouseManagementSystem.Data.WarehouseDbContext))]
[Migration("20260402000000_AddEmployeeIdAndItemNumberLookup")]
public partial class AddEmployeeIdAndItemNumberLookup : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "EmployeeId",
            table: "TeamMembers",
            type: "int",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.CreateIndex(
            name: "IX_TeamMembers_EmployeeId",
            table: "TeamMembers",
            column: "EmployeeId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InventoryItems_ItemNumber",
            table: "InventoryItems",
            column: "ItemNumber",
            unique: true);

        migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM [TeamMembers] WHERE [EmployeeId] = 1)
            BEGIN
                INSERT INTO [TeamMembers] ([EmployeeId], [FullName], [Birthdate], [CollegeProgram], [YearInProgram], [Email])
                VALUES (1, 'Mohamed Njie', '2007-07-02', 'IT Software Development', 2, 'njiemt@mail.uc.edu');
            END
            """);

    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM [TeamMembers] WHERE [EmployeeId] = 1;");

        migrationBuilder.DropIndex(
            name: "IX_TeamMembers_EmployeeId",
            table: "TeamMembers");

        migrationBuilder.DropIndex(
            name: "IX_InventoryItems_ItemNumber",
            table: "InventoryItems");

        migrationBuilder.DropColumn(
            name: "EmployeeId",
            table: "TeamMembers");
    }
}