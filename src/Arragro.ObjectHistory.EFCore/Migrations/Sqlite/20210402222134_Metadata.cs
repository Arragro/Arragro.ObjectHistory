using Microsoft.EntityFrameworkCore.Migrations;

namespace Arragro.ObjectHistory.EFCore.Migrations.Sqlite
{
    public partial class Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "metadata",
                schema: "object_history",
                table: "object_history_table_entity",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                schema: "object_history",
                table: "object_history_global_table_entity",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
                schema: "object_history",
                table: "object_history_table_entity");

            migrationBuilder.DropColumn(
                name: "metadata",
                schema: "object_history",
                table: "object_history_global_table_entity");

            migrationBuilder.DropColumn(
                name: "metadata",
                schema: "object_history",
                table: "object_history_deleted_table_entities");
        }
    }
}
