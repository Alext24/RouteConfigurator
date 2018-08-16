using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_EngineeredModels
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
