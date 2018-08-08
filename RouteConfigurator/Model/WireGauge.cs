using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("WireGauge")]
    public class WireGauge
    {
        [Key]
        public string Gauge { get; set; }

        [Required(ErrorMessage = "Time Percentage is Required")]
        public decimal TimePercentage { get; set; }
    }
}
