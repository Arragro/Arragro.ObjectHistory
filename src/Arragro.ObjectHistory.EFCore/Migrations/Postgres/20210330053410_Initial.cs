using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arragro.ObjectHistory.EFCore.Migrations.Postgres
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "object_history");

            migrationBuilder.CreateTable(
                name: "ObjectHistoryDeletedTableEntities",
                schema: "object_history",
                columns: table => new
                {
                    PartitionKey = table.Column<string>(type: "text", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false),
                    ApplicationName = table.Column<string>(type: "text", nullable: true),
                    Folder = table.Column<Guid>(type: "uuid", nullable: false),
                    SubFolder = table.Column<Guid>(type: "uuid", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SecurityValidationToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectHistoryDeletedTableEntities", x => new { x.PartitionKey, x.RowKey });
                });

            migrationBuilder.CreateTable(
                name: "ObjectHistoryGlobalTableEntity",
                schema: "object_history",
                columns: table => new
                {
                    PartitionKey = table.Column<string>(type: "text", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false),
                    User = table.Column<string>(type: "text", nullable: true),
                    ObjectName = table.Column<string>(type: "text", nullable: true),
                    Folder = table.Column<Guid>(type: "uuid", nullable: false),
                    SubFolder = table.Column<Guid>(type: "uuid", nullable: true),
                    IsAdd = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SecurityValidationToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectHistoryGlobalTableEntity", x => new { x.PartitionKey, x.RowKey });
                });

            migrationBuilder.CreateTable(
                name: "ObjectHistoryTableEntity",
                schema: "object_history",
                columns: table => new
                {
                    PartitionKey = table.Column<string>(type: "text", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false),
                    ApplicationName = table.Column<string>(type: "text", nullable: true),
                    Folder = table.Column<Guid>(type: "uuid", nullable: false),
                    SubFolder = table.Column<Guid>(type: "uuid", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    IsAdd = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SecurityValidationToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectHistoryTableEntity", x => new { x.PartitionKey, x.RowKey });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHistoryDeletedTableEntities_RowKey",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                column: "RowKey");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHistoryGlobalTableEntity_RowKey",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                column: "RowKey");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectHistoryTableEntity_RowKey",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                column: "RowKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectHistoryDeletedTableEntities",
                schema: "object_history");

            migrationBuilder.DropTable(
                name: "ObjectHistoryGlobalTableEntity",
                schema: "object_history");

            migrationBuilder.DropTable(
                name: "ObjectHistoryTableEntity",
                schema: "object_history");
        }
    }
}
