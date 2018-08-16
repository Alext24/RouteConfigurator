using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_StandardModels
{
    [Table("Model")]
    public class StandardModel
    {
        [Key]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Base has to be 8 characters")]
        public string Base { get; set; }

        [Required(ErrorMessage = "Box Size is Required")]
        [Display(Name = "Box Size")]
        [StringLength(5, MinimumLength = 2)]
        public string BoxSize { get; set; }

        [Required(ErrorMessage = "Drive Time is Required")]
        [Display(Name = "Drive Time")]
        public decimal DriveTime { get; set; }

        [Required(ErrorMessage = "AV Time is Required")]
        [Display(Name = "AV Time")]
        public decimal AVTime { get; set; }

        [Required(ErrorMessage = "Product Line is Required")]
        [Display(Name = "Product Line")]
        public string Line { get; set; }

        public virtual ICollection<Override> Overrides { get; set; }

        public virtual ICollection<TimeTrial> TimeTrials { get; set; }
    }
}
