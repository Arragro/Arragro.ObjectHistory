using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arragro.ObjectHistory.EFCore.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class SnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjectHistoryTableEntity",
                schema: "object_history",
                table: "ObjectHistoryTableEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjectHistoryGlobalTableEntity",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjectHistoryDeletedTableEntities",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities");

            migrationBuilder.RenameTable(
                name: "ObjectHistoryTableEntity",
                schema: "object_history",
                newName: "object_history_table_entity",
                newSchema: "object_history");

            migrationBuilder.RenameTable(
                name: "ObjectHistoryGlobalTableEntity",
                schema: "object_history",
                newName: "object_history_global_table_entity",
                newSchema: "object_history");

            migrationBuilder.RenameTable(
                name: "ObjectHistoryDeletedTableEntities",
                schema: "object_history",
                newName: "object_history_deleted_table_entities",
                newSchema: "object_history");

            migrationBuilder.RenameColumn(
                name: "Version",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "User",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "user");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "metadata");

            migrationBuilder.RenameColumn(
                name: "Folder",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "folder");

            migrationBuilder.RenameColumn(
                name: "SubFolder",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "sub_folder");

            migrationBuilder.RenameColumn(
                name: "SecurityValidationToken",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "security_validation_token");

            migrationBuilder.RenameColumn(
                name: "IsAdd",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "is_add");

            migrationBuilder.RenameColumn(
                name: "ApplicationName",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "application_name");

            migrationBuilder.RenameColumn(
                name: "RowKey",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "row_key");

            migrationBuilder.RenameColumn(
                name: "PartitionKey",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "partition_key");

            migrationBuilder.RenameIndex(
                name: "IX_ObjectHistoryTableEntity_RowKey",
                schema: "object_history",
                table: "object_history_table_entity",
                newName: "ix_object_history_table_entity_row_key");

            migrationBuilder.RenameColumn(
                name: "Version",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "User",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "user");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "metadata");

            migrationBuilder.RenameColumn(
                name: "Folder",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "folder");

            migrationBuilder.RenameColumn(
                name: "SubFolder",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "sub_folder");

            migrationBuilder.RenameColumn(
                name: "SecurityValidationToken",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "security_validation_token");

            migrationBuilder.RenameColumn(
                name: "ObjectName",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "object_name");

            migrationBuilder.RenameColumn(
                name: "IsAdd",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "is_add");

            migrationBuilder.RenameColumn(
                name: "RowKey",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "row_key");

            migrationBuilder.RenameColumn(
                name: "PartitionKey",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "partition_key");

            migrationBuilder.RenameIndex(
                name: "IX_ObjectHistoryGlobalTableEntity_RowKey",
                schema: "object_history",
                table: "object_history_global_table_entity",
                newName: "ix_object_history_global_table_entity_row_key");

            migrationBuilder.RenameColumn(
                name: "Version",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "User",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "user");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "metadata");

            migrationBuilder.RenameColumn(
                name: "Folder",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "folder");

            migrationBuilder.RenameColumn(
                name: "SubFolder",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "sub_folder");

            migrationBuilder.RenameColumn(
                name: "SecurityValidationToken",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "security_validation_token");

            migrationBuilder.RenameColumn(
                name: "ApplicationName",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "application_name");

            migrationBuilder.RenameColumn(
                name: "RowKey",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "row_key");

            migrationBuilder.RenameColumn(
                name: "PartitionKey",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "partition_key");

            migrationBuilder.RenameIndex(
                name: "IX_ObjectHistoryDeletedTableEntities_RowKey",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                newName: "ix_object_history_deleted_table_entities_row_key");

            migrationBuilder.AddPrimaryKey(
                name: "pk_object_history_table_entity",
                schema: "object_history",
                table: "object_history_table_entity",
                columns: new[] { "partition_key", "row_key" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_object_history_global_table_entity",
                schema: "object_history",
                table: "object_history_global_table_entity",
                columns: new[] { "partition_key", "row_key" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_object_history_deleted_table_entities",
                schema: "object_history",
                table: "object_history_deleted_table_entities",
                columns: new[] { "partition_key", "row_key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_object_history_table_entity",
                schema: "object_history",
                table: "object_history_table_entity");

            migrationBuilder.DropPrimaryKey(
                name: "pk_object_history_global_table_entity",
                schema: "object_history",
                table: "object_history_global_table_entity");

            migrationBuilder.DropPrimaryKey(
                name: "pk_object_history_deleted_table_entities",
                schema: "object_history",
                table: "object_history_deleted_table_entities");

            migrationBuilder.RenameTable(
                name: "object_history_table_entity",
                schema: "object_history",
                newName: "ObjectHistoryTableEntity",
                newSchema: "object_history");

            migrationBuilder.RenameTable(
                name: "object_history_global_table_entity",
                schema: "object_history",
                newName: "ObjectHistoryGlobalTableEntity",
                newSchema: "object_history");

            migrationBuilder.RenameTable(
                name: "object_history_deleted_table_entities",
                schema: "object_history",
                newName: "ObjectHistoryDeletedTableEntities",
                newSchema: "object_history");

            migrationBuilder.RenameColumn(
                name: "version",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "user",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "metadata",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "folder",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "Folder");

            migrationBuilder.RenameColumn(
                name: "sub_folder",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "SubFolder");

            migrationBuilder.RenameColumn(
                name: "security_validation_token",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "SecurityValidationToken");

            migrationBuilder.RenameColumn(
                name: "is_add",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "IsAdd");

            migrationBuilder.RenameColumn(
                name: "application_name",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "ApplicationName");

            migrationBuilder.RenameColumn(
                name: "row_key",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "RowKey");

            migrationBuilder.RenameColumn(
                name: "partition_key",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "PartitionKey");

            migrationBuilder.RenameIndex(
                name: "ix_object_history_table_entity_row_key",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                newName: "IX_ObjectHistoryTableEntity_RowKey");

            migrationBuilder.RenameColumn(
                name: "version",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "user",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "metadata",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "folder",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "Folder");

            migrationBuilder.RenameColumn(
                name: "sub_folder",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "SubFolder");

            migrationBuilder.RenameColumn(
                name: "security_validation_token",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "SecurityValidationToken");

            migrationBuilder.RenameColumn(
                name: "object_name",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "ObjectName");

            migrationBuilder.RenameColumn(
                name: "is_add",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "IsAdd");

            migrationBuilder.RenameColumn(
                name: "row_key",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "RowKey");

            migrationBuilder.RenameColumn(
                name: "partition_key",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "PartitionKey");

            migrationBuilder.RenameIndex(
                name: "ix_object_history_global_table_entity_row_key",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                newName: "IX_ObjectHistoryGlobalTableEntity_RowKey");

            migrationBuilder.RenameColumn(
                name: "version",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "user",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "metadata",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "folder",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "Folder");

            migrationBuilder.RenameColumn(
                name: "sub_folder",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "SubFolder");

            migrationBuilder.RenameColumn(
                name: "security_validation_token",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "SecurityValidationToken");

            migrationBuilder.RenameColumn(
                name: "application_name",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "ApplicationName");

            migrationBuilder.RenameColumn(
                name: "row_key",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "RowKey");

            migrationBuilder.RenameColumn(
                name: "partition_key",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "PartitionKey");

            migrationBuilder.RenameIndex(
                name: "ix_object_history_deleted_table_entities_row_key",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                newName: "IX_ObjectHistoryDeletedTableEntities_RowKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObjectHistoryTableEntity",
                schema: "object_history",
                table: "ObjectHistoryTableEntity",
                columns: new[] { "PartitionKey", "RowKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObjectHistoryGlobalTableEntity",
                schema: "object_history",
                table: "ObjectHistoryGlobalTableEntity",
                columns: new[] { "PartitionKey", "RowKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObjectHistoryDeletedTableEntities",
                schema: "object_history",
                table: "ObjectHistoryDeletedTableEntities",
                columns: new[] { "PartitionKey", "RowKey" });
        }
    }
}
