﻿// <auto-generated />
using System;
using Arragro.ObjectHistory.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Arragro.ObjectHistory.EFCore.Migrations.SqlServer
{
    [DbContext(typeof(ArragroObjectHistoryContext))]
    partial class ArragroObjectHistoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("object_history")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Arragro.ObjectHistory.EFCore.ObjectHistoryDeletedTableEntity", b =>
                {
                    b.Property<string>("PartitionKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("RowKey")
                        .HasColumnType("bigint");

                    b.Property<string>("ApplicationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Folder")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SecurityValidationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SubFolder")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("PartitionKey", "RowKey");

                    b.HasIndex("RowKey");

                    b.ToTable("ObjectHistoryDeletedTableEntities");
                });

            modelBuilder.Entity("Arragro.ObjectHistory.EFCore.ObjectHistoryGlobalTableEntity", b =>
                {
                    b.Property<string>("PartitionKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("RowKey")
                        .HasColumnType("bigint");

                    b.Property<Guid>("Folder")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAdd")
                        .HasColumnType("bit");

                    b.Property<string>("ObjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityValidationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SubFolder")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("PartitionKey", "RowKey");

                    b.HasIndex("RowKey");

                    b.ToTable("ObjectHistoryGlobalTableEntity");
                });

            modelBuilder.Entity("Arragro.ObjectHistory.EFCore.ObjectHistoryTableEntity", b =>
                {
                    b.Property<string>("PartitionKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("RowKey")
                        .HasColumnType("bigint");

                    b.Property<string>("ApplicationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Folder")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAdd")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityValidationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SubFolder")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("PartitionKey", "RowKey");

                    b.HasIndex("RowKey");

                    b.ToTable("ObjectHistoryTableEntity");
                });
#pragma warning restore 612, 618
        }
    }
}
