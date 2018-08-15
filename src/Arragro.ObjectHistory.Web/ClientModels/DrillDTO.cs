using Arragro.ObjectHistory.RazorClassLib.Helpers;
using Arragro.ObjectHistory.Web.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.ClientModels
{
    public class DrillDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int Duration { get; set; }
    }

    public class SessionDrillContainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string ObjectHistoryKey { get; set; }

        public List<DrillDto> Drills { get; set; } = new List<DrillDto>();

        public SessionDrillContainer(TrainingSession trainingSession, List<DrillDto> drills)
        {
            Id = trainingSession.Id;
            Name = trainingSession.Name;
            DateCreated = trainingSession.DateCreated;
            ObjectHistoryKey = ObjectHistoryHelper.GetObjectHistoryFullNameAndId(typeof(TrainingSession), Id.ToString());

            Drills = drills;
        }
    }
}
