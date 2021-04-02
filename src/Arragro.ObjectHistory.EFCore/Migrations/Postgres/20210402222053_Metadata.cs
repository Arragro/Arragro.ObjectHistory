using Microsoft.EntityFrameworkCore.Migrations;

namespace Arragro.ObjectHistory.EFCore.Migrations.Postgres
{
    public partial class Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                type: "text",
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
