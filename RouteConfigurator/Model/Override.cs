﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("Override")]
    public class Override
    {
        [Key]
        [StringLength(64, MinimumLength = 8)]
        [Display(Name = "Model Number")]
        public string ModelNum { get; set; }

        [Required(ErrorMessage = "Override Route is Required")]
        [Display(Name = "Override Route")]
        public int OverrideRoute { get; set; }

        [Required(ErrorMessage = "Override Time is Required")]
        [Display(Name = "Override Time")]
        public decimal OverrideTime { get; set; }
        
        public virtual Model Model { get; set; }
    }
}
