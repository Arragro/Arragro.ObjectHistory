using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.Models
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
