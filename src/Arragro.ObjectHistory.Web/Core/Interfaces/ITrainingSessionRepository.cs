using Arragro.ObjectHistory.Web.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Core.Interfaces
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
