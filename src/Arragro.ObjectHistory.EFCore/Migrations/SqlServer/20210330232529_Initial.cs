using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Arragro.ObjectHistory.EFCore.Migrations.SqlServer
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
                    PartitionKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Folder = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubFolder = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SecurityValidationToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    PartitionKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Folder = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubFolder = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsAdd = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SecurityValidationToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    PartitionKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RowKey = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Folder = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubFolder = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdd = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SecurityValidationToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
