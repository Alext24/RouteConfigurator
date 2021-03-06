﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_StandardModels
{
    [Table("TimeTrialsOptionTime")]
    public class TimeTrialsOptionTime
    {
        [Key]
        [Column(Order=1)]
        [StringLength(2)]
        [Display(Name = "Option Code")]
        public string OptionCode { get; set; }

        [Key]
        [Column(Order=2)]
        [Display(Name = "Production Number")]
        public int ProductionNumber { get; set; }

        [Required(ErrorMessage = "Option Time is Required")]
        [Display(Name = "Option Time")]
        public decimal Time { get; set; }

        public virtual TimeTrial TimeTrial { get; set; }
    }
}
