using Arragro.Core.EntityFrameworkCore;
using Arragro.Core.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace Arragro.ObjectHistory.EFCore
{
    public class ArragroObjectHistoryBaseContext : BaseContext
    {
        public DbSet<ObjectHistoryDeletedTableEntity> ObjectHistoryDeletedTableEntities { get; set; }
        public DbSet<ObjectHistoryGlobalTableEntity> ObjectHistoryGlobalTableEntity { get; set; }
        public DbSet<ObjectHistoryTableEntity> ObjectHistoryTableEntity { get; set; }

        public ArragroObjectHistoryBaseContext(DbContextOptions options)
            : base(options)
        {
            ChangeTracker.CascadeDeleteTiming = CascadeTiming.Never;
            ChangeTracker.DeleteOrphansTiming = CascadeTiming.Never;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("object_history");

            modelBuilder.Entity<ObjectHistoryDeletedTableEntity>()
                .HasKey(x => new { x.PartitionKey, x.RowKey });

            modelBuilder.Entity<ObjectHistoryDeletedTableEntity>()
                .HasIndex(x => x.RowKey);

            modelBuilder.Entity<ObjectHistoryGlobalTableEntity>()
                .HasKey(x => new { x.PartitionKey, x.RowKey });

            modelBuilder.Entity<ObjectHistoryGlobalTableEntity>()
                .HasIndex(x => x.RowKey);

            modelBuilder.Entity<ObjectHistoryTableEntity>()
                .HasKey(x => new { x.PartitionKey, x.RowKey });

            modelBuilder.Entity<ObjectHistoryTableEntity>()
                .HasIndex(x => x.RowKey);


            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite") return;

            modelBuilder.SnakeCaseTablesAndProperties();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset));
                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }
    }

    public class ArragroObjectHistoryBaseContext<T> : ArragroObjectHistoryBaseContext where T : DbContext
    {

        public ArragroObjectHistoryBaseContext() : base(new DbContextOptions<T>())
        {
        }

        public ArragroObjectHistoryBaseContext(DbContextOptions<T> options)
            : base(options)
        {
        }
    }

    public class ArragroObjectHistoryContext : ArragroObjectHistoryBaseContext<ArragroObjectHistoryContext>
    {
        public ArragroObjectHistoryContext() { }

        public ArragroObjectHistoryContext(DbContextOptions<ArragroObjectHistoryContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=arragro-object-history;Trusted_Connection=True;");
            }
        }
    }

    public class ArragroObjectHistoryPGContext : ArragroObjectHistoryBaseContext<ArragroObjectHistoryPGContext>
    {
        public ArragroObjectHistoryPGContext() { }

        public ArragroObjectHistoryPGContext(DbContextOptions<ArragroObjectHistoryPGContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"host=localhost;port=5432;database=arragro-object-history;user id=postgres;password=Password1");
            }
        }
    }

    public class ArragroObjectHistorySqliteContext : ArragroObjectHistoryBaseContext<ArragroObjectHistorySqliteContext>
    {
        public ArragroObjectHistorySqliteContext() { }

        public ArragroObjectHistorySqliteContext(DbContextOptions<ArragroObjectHistorySqliteContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"DataSource=:memory:");
            }
        }
    }
}
