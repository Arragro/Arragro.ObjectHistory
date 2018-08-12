using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.ClientModels
{
    public class DrillDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int Duration { get; set; }
    }
}
