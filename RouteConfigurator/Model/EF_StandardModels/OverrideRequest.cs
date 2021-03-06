﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_StandardModels
{
    [Table("OverrideRequest")]
    public class OverrideRequest
    {
        [Key]
        [Display(Name = "Override Request ID")]
        public int OverrideRequestID { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Request Date")]
        [Column(TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Review Date")]
        [Column(TypeName = "date")]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Description")]
        [Column(TypeName = "text")]
        public string Description { get; set; }

        /// <summary>
        /// 0 - waiting
        /// 1 - approved
        /// 2 - denied
        /// 3 - currently approved checked
        /// 4 - currently denied checked
        /// </summary>
        [Required]
        public int State { get; set; }

        [Required]
        public string Sender { get; set; }

        public string Reviewer { get; set; }

        [StringLength(64, MinimumLength = 8)]
        [Display(Name = "Model Number")]
        public string ModelNum { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Base has to be 8 characters")]
        [Display(Name = "Model Base")]
        public virtual string ModelBase { get; set; }

        [Required(ErrorMessage = "Override Time is Required")]
        [Display(Name = "Override Time")]
        public decimal OverrideTime { get; set; }

        [Required(ErrorMessage = "Override Route is Required")]
        [Display(Name = "Override Route")]
        public int OverrideRoute { get; set; }
        
        [Required(ErrorMessage = "Model Time is Required")]
        [Display(Name = "Model Time")]
        public decimal ModelTime { get; set; }

        [Required(ErrorMessage = "Model Route is Required")]
        [Display(Name = "Model Route")]
        public int ModelRoute { get; set; }
    }
}
