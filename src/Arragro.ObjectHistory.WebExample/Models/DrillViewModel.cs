using System;

namespace Arragro.ObjectHistory.WebExample.Models
{
    public class DrillViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
