using Arragro.ObjectHistory.WebExample.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arragro.ObjectHistory.WebExample.Infrastructure
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
    }
}
