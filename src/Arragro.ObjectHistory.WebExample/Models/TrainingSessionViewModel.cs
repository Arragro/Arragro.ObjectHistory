using Arragro.ObjectHistory.Web.Helpers;
using Arragro.ObjectHistory.WebExample.Core.Entities;
using System;

namespace Arragro.ObjectHistory.WebExample.Models
{
    public class TrainingSessionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int DrillCount { get; set; }
        public string ObjectHistoryFullNameAndId
        {
            get
            {
                return ObjectHistoryHelper.GetObjectHistoryFullNameAndId(typeof(TrainingSession), Id.ToString());
            }
        }
    }
}
