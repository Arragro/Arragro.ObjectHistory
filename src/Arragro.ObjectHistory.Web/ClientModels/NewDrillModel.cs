using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Web.ClientModels
{
    public class NewDrillModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Duration { get; set; }

        [Range(1, 1000000)]
        public int SessionId { get; set; }
    }
}
