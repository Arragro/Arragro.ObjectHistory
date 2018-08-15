using Arragro.ObjectHistory.RazorClassLib.Helpers;
using Arragro.ObjectHistory.Web.Core.Entities;
using System;

namespace Arragro.ObjectHistory.Web.Models
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
