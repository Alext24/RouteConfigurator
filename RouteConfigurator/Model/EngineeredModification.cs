using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("EngineeredModification")]
    public class EngineeredModification
    {
        [Key]
        [Display(Name = "Modification ID")]
        public int ModificationID { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Request Date")]
        [Column(TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Reviewed Date")]
        [Column(TypeName = "date")]
        public DateTime ReviewedDate { get; set; }

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
        /// False means modifying old item
        /// True means adding new item
        /// </summary>
        [Required]
        public bool IsNew { get; set; }

        public string ComponentName { get; set; }

        public string EnclosureSize { get; set; }

        public string EnclosureType { get; set; }

        public decimal NewTime { get; set; }

        public decimal OldTime { get; set; }

        public string Gauge { get; set; }

        public decimal NewTimePercentage { get; set; }

        public decimal OldTimePercentage { get; set; }
    }
}
