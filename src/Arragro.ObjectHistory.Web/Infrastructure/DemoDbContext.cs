using Arragro.ObjectHistory.Web.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Infrastructure
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
    }
}
