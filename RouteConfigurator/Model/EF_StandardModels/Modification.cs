using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteConfigurator.Model.EF_StandardModels
{
    [Table("Modification")]
    public class Modification
    {
        [Key]
        [Display(Name = "Modification ID")]
        public int ModificationID { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Request Date")]
        [Column(TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Review Date")]
        [Column(TypeName = "date")]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Description")]
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

        /// <summary>
        /// False means model
        /// True means option
        /// </summary>
        [Required]
        public bool IsOption { get; set; }

        /// <summary>
        /// False means modifying old item
        /// True means new item
        /// </summary>
        [Required]
        public bool IsNew { get; set; }

        /// <summary>
        /// Used for both model and option
        /// </summary>
        [StringLength(5, MinimumLength = 2)]
        public string BoxSize { get; set; }

        [StringLength(8, ErrorMessage = "Base has to be 8 characters")]
        public string ModelBase { get; set; }

        public decimal NewDriveTime { get; set; }
        public decimal NewAVTime { get; set; }

        public decimal OldModelDriveTime { get; set; }
        public decimal OldModelAVTime { get; set; }

        [StringLength(2, ErrorMessage = "Option Code format must be 'PX' or 'TX'")]
        public string OptionCode { get; set; }

        public decimal NewTime { get; set; }
        [StringLength(100)]
        public string NewName { get; set; }

        public decimal OldOptionTime { get; set; }
        [StringLength(100)]
        public string OldOptionName { get; set; }

        public string ProductLine { get; set; }
    }
}
