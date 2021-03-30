using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arragro.ObjectHistory.EFCore.Migrations.Sqlite
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "object_history");

            migrationBuilder.CreateTable(
                name: "object_history_deleted_table_entities",
                schema: "object_history",
                columns: table => new
                {
                    partition_key = table.Column<string>(type: "TEXT", nullable: false),
                    row_key = table.Column<long>(type: "INTEGER", nullable: false),
                    application_name = table.Column<string>(type: "TEXT", nullable: true),
                    folder = table.Column<Guid>(type: "TEXT", nullable: false),
                    sub_folder = table.Column<Guid>(type: "TEXT", nullable: true),
                    user = table.Column<string>(type: "TEXT", nullable: true),
                    timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    security_validation_token = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_object_history_deleted_table_entities", x => new { x.partition_key, x.row_key });
                });

            migrationBuilder.CreateTable(
                name: "object_history_global_table_entity",
                schema: "object_history",
                columns: table => new
                {
                    partition_key = table.Column<string>(type: "TEXT", nullable: false),
                    row_key = table.Column<long>(type: "INTEGER", nullable: false),
                    user = table.Column<string>(type: "TEXT", nullable: true),
                    object_name = table.Column<string>(type: "TEXT", nullable: true),
                    folder = table.Column<Guid>(type: "TEXT", nullable: false),
                    sub_folder = table.Column<Guid>(type: "TEXT", nullable: true),
                    is_add = table.Column<bool>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    security_validation_token = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_object_history_global_table_entity", x => new { x.partition_key, x.row_key });
                });

            migrationBuilder.CreateTable(
                name: "object_history_table_entity",
                schema: "object_history",
                columns: table => new
                {
                    partition_key = table.Column<string>(type: "TEXT", nullable: false),
                    row_key = table.Column<long>(type: "INTEGER", nullable: false),
                    application_name = table.Column<string>(type: "TEXT", nullable: true),
                    folder = table.Column<Guid>(type: "TEXT", nullable: false),
                    sub_folder = table.Column<Guid>(type: "TEXT", nullable: true),
                    user = table.Column<string>(type: "TEXT", nullable: true),
                    is_add = table.Column<bool>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    security_validation_token = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_object_history_table_entity", x => new { x.partition_key, x.row_key });
                });

            migrationBuilder.CreateIndex(
                name: "ix_object_history_deleted_table_entities_row_key",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                column: "row_key");

            migrationBuilder.CreateIndex(
                name: "ix_object_history_global_table_entity_row_key",
                schema: "object_history",
                table: "object_history_global_table_entity",
                column: "row_key");

            migrationBuilder.CreateIndex(
                name: "ix_object_history_table_entity_row_key",
                schema: "object_history",
                table: "object_history_table_entity",
                column: "row_key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "object_history_deleted_table_entities",
                schema: "object_history");

            migrationBuilder.DropTable(
                name: "object_history_global_table_entity",
                schema: "object_history");

            migrationBuilder.DropTable(
                name: "object_history_table_entity",
                schema: "object_history");
        }
    }
}
