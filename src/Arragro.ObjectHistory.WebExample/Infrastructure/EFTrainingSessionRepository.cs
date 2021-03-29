using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.WebExample.Core.Entities;
using Arragro.ObjectHistory.WebExample.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.WebExample.Infrastructure
{
    public class EFTrainingSessionRepository : ITrainingSessionRepository
    {
        private readonly DemoDbContext _dbContext;
        private readonly IObjectHistoryClient _objectHistoryClient;

        public EFTrainingSessionRepository(DemoDbContext dbContext, IObjectHistoryClient objectHistoryClient)
        {
            _dbContext = dbContext;
            _objectHistoryClient = objectHistoryClient;
        }

        public async Task<TrainingSession> GetByIdNoTrackingAsync(int id)
        {
            return await _dbContext.TrainingSessions
                .Include(s => s.Drills)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<TrainingSession> GetByIdAsync(int id)
        {
            return await _dbContext.TrainingSessions
                .Include(s => s.Drills)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<TrainingSession>> ListAsync()
        {
            return await _dbContext.TrainingSessions
                .Include(s => s.Drills)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }

        public async Task AddAsync(TrainingSession session)
        {
            try
            {
                _dbContext.TrainingSessions.Add(session);
                await _dbContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }

            await _objectHistoryClient.QueueObjectHistoryAsync<TrainingSession>(() => $"{session.Id}", session, "User1");         
        }

        public async Task UpdateAsync(TrainingSession session, TrainingSession unmodifiedSession)
        {
            _dbContext.Entry(session).State = EntityState.Modified;
            
            try
            {
                var task = await _dbContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }

            session = await GetByIdNoTrackingAsync(session.Id);
            await _objectHistoryClient.SaveObjectHistoryAsync<TrainingSession>(() => $"{session.Id}", session, "User1");
        }
    }
}
