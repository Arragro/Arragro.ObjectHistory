using Arragro.ObjectHistory.WebExample.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.WebExample.Core.Interfaces
{
    public interface ITrainingSessionRepository
    {
        Task<TrainingSession> GetByIdNoTrackingAsync(int id);
        Task<TrainingSession> GetByIdAsync(int id);
        Task<List<TrainingSession>> ListAsync();
        Task AddAsync(TrainingSession session);
        Task UpdateAsync(TrainingSession session, TrainingSession unmodifiedSession);
    }
}
