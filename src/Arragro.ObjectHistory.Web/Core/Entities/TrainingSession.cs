using System;
using System.Collections.Generic;

namespace Arragro.ObjectHistory.Web.Core.Entities
{
    public class TrainingSession
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public List<Drill> Drills { get; set; } = new List<Drill>();

        public void AddDrill(Drill drill)
        {
            Drills.Add(drill);
        }
    }

    public class Drill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime DateCreated { get; set; }
        public Difficulty SkillLevel { get; set; }
    }

    public enum Difficulty
    {
        Beginner,
        Expert
    }
}
