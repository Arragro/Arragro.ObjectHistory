using Microsoft.EntityFrameworkCore.Migrations;

namespace Arragro.ObjectHistory.EFCore.Migrations.SqlServer
{
    public partial class Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryTableEntity");

            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity");

            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities");
        }
    }
}
