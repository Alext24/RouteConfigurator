using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_StandardModels
{
    [Table("Option")]
    public class Option
    {
        [Key]
        [Column(Order=1)]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Option Code format must be 'PX' or 'TX'")]
        [Display(Name = "Option Code")]
        public string OptionCode { get; set; }

        [Key]
        [Column(Order=2)]
        [StringLength(5, MinimumLength = 2)]
        [Display(Name = "Box Size")]
        public string BoxSize { get; set; }

        [Required(ErrorMessage = "Option Time is Required")]
        [Display(Name = "Option Time")]
        public decimal Time { get; set; }

        [StringLength(100)]
        [Display(Name = "Option Name/Description")]
        public string Name { get; set; }
    }
}
