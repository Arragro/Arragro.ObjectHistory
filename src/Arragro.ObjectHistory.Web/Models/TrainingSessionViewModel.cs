using System;

namespace Arragro.ObjectHistory.Web.Models
{
    public class TrainingSessionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int DrillCount { get; set; }
    }
}
