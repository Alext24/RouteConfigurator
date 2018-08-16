using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_EngineeredModels
{
    [Table("Enclosure")]
    public class Enclosure
    {
        [Key]
        [Column(Order = 1)]
        public string EnclosureType{ get; set; }

        [Key]
        [Column(Order = 2)]
        public string EnclosureSize{ get; set; }

        [Required(ErrorMessage = "Time is Required")]
        public decimal Time { get; set; }
    }
}
