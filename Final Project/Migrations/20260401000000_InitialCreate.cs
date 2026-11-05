using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations;

[DbContext(typeof(WarehouseManagementSystem.Data.WarehouseDbContext))]
[Migration("20260401000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Locations",
            columns: table => new
            {
                LocationId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                LocationCode = table.Column<string>(maxLength: 20, nullable: false),
                Aisle = table.Column<string>(maxLength: 10, nullable: false),
                Rack = table.Column<string>(maxLength: 10, nullable: false),
                Shelf = table.Column<string>(maxLength: 10, nullable: false),
                LocationDescription = table.Column<string>(maxLength: 150, nullable: true),
                IsActive = table.Column<bool>(nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Locations", x => x.LocationId));

        migrationBuilder.CreateTable(
            name: "TeamMembers",
            columns: table => new
            {
                MemberId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(maxLength: 150, nullable: false),
                Birthdate = table.Column<DateTime>(nullable: false),
                CollegeProgram = table.Column<string>(maxLength: 100, nullable: false),
                YearInProgram = table.Column<int>(nullable: false),
                Email = table.Column<string>(maxLength: 150, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_TeamMembers", x => x.MemberId));

        migrationBuilder.CreateTable(
            name: "InventoryItems",
            columns: table => new
            {
                ItemId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ItemNumber = table.Column<string>(maxLength: 9, nullable: false),
                ItemName = table.Column<string>(maxLength: 100, nullable: false),
                ItemDescription = table.Column<string>(maxLength: 500, nullable: true),
                LocationId = table.Column<int>(nullable: false),
                ConditionName = table.Column<string>(maxLength: 30, nullable: false),
                Quantity = table.Column<int>(nullable: false),
                ReorderLevel = table.Column<int>(nullable: false),
                DateAdded = table.Column<DateTime>(nullable: false),
                LastUpdated = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryItems", x => x.ItemId);
                table.ForeignKey("FK_InventoryItems_Locations_LocationId", x => x.LocationId, "Locations", "LocationId", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "InventoryComments",
            columns: table => new
            {
                CommentId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ItemId = table.Column<int>(nullable: false),
                CommentText = table.Column<string>(maxLength: 1000, nullable: false),
                CommentDate = table.Column<DateTime>(nullable: false),
                CommentedBy = table.Column<string>(maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryComments", x => x.CommentId);
                table.ForeignKey("FK_InventoryComments_InventoryItems_ItemId", x => x.ItemId, "InventoryItems", "ItemId", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "InventoryTransactions",
            columns: table => new
            {
                TransactionId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ItemId = table.Column<int>(nullable: false),
                TransactionType = table.Column<string>(maxLength: 20, nullable: false),
                QuantityBefore = table.Column<int>(nullable: true),
                QuantityAfter = table.Column<int>(nullable: true),
                ConditionBefore = table.Column<string>(maxLength: 30, nullable: true),
                ConditionAfter = table.Column<string>(maxLength: 30, nullable: true),
                TransactionDate = table.Column<DateTime>(nullable: false),
                ChangedBy = table.Column<string>(maxLength: 128, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryTransactions", x => x.TransactionId);
                table.ForeignKey("FK_InventoryTransactions_InventoryItems_ItemId", x => x.ItemId, "InventoryItems", "ItemId", onDelete: ReferentialAction.Cascade);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "InventoryComments");
        migrationBuilder.DropTable(name: "InventoryTransactions");
        migrationBuilder.DropTable(name: "InventoryItems");
        migrationBuilder.DropTable(name: "TeamMembers");
        migrationBuilder.DropTable(name: "Locations");
    }
}