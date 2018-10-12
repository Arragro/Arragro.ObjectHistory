using System;
using System.ComponentModel.DataAnnotations;

namespace Arragro.ObjectHistory.WebExample.ClientModels
{
    public class NewDrillModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        public int Duration { get; set; }

        [Range(1, 1000000)]
        public int SessionId { get; set; }
    }
}
