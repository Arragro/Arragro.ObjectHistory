using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Web.Core.Entities;
using Arragro.ObjectHistory.Web.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Infrastructure
{
    public class EFTrainingSessionRepository : ITrainingSessionRepository
    {
        private readonly DemoDbContext _dbContext;
        private readonly ObjectHistoryClient _objectHistoryClient;

        public EFTrainingSessionRepository(DemoDbContext dbContext, ObjectHistoryClient objectHistoryClient)
        {
            _dbContext = dbContext;
            _objectHistoryClient = objectHistoryClient;
        }

        public Task<TrainingSession> GetByIdNoTrackingAsync(int id)
        {
            return _dbContext.TrainingSessions
                .Include(s => s.Drills)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<TrainingSession> GetByIdAsync(int id)
        {
            return _dbContext.TrainingSessions
                .Include(s => s.Drills)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<List<TrainingSession>> ListAsync()
        {
            return _dbContext.TrainingSessions
                .Include(s => s.Drills)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }

        public Task AddAsync(TrainingSession session)
        {
            _dbContext.TrainingSessions.Add(session);
            _dbContext.SaveChangesAsync();
            return _objectHistoryClient.SaveNewObjectHistoryAsync<TrainingSession>(() => $"{session.Id}", session, "User1");
            
        }

        public Task UpdateAsync(TrainingSession session, TrainingSession unmodifiedSession)
        {
            _dbContext.Entry(session).State = EntityState.Modified;
            
            try
            {
                _dbContext.SaveChangesAsync();
                return _objectHistoryClient.SaveObjectHistoryAsync<TrainingSession>(() => $"{session.Id}", unmodifiedSession, session, "User1");
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
