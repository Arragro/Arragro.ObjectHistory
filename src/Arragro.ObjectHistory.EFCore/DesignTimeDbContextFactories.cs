using Microsoft.EntityFrameworkCore.Design;

namespace Arragro.ObjectHistory.EFCore
{
    public class ArragroObjectHistoryContextFactory : IDesignTimeDbContextFactory<ArragroObjectHistoryContext>
    {
        public ArragroObjectHistoryContext CreateDbContext(string[] args)
        {
            return new ArragroObjectHistoryContext();
        }
    }

    public class ArragroObjectHistoryPGContextFactory : IDesignTimeDbContextFactory<ArragroObjectHistoryPGContext>
    {
        public ArragroObjectHistoryPGContext CreateDbContext(string[] args)
        {
            return new ArragroObjectHistoryPGContext();
        }
    }

    public class ArragroObjectHistorySqliteContextFactory : IDesignTimeDbContextFactory<ArragroObjectHistorySqliteContext>
    {
        public ArragroObjectHistorySqliteContext CreateDbContext(string[] args)
        {
            return new ArragroObjectHistorySqliteContext();
        }
    }
}
