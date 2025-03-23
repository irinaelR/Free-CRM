using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AlertManager
{
    public class AlertConfigRequest
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        public double Percentage { get; set; }
    }
}
