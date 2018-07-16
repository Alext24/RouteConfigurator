using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("TimeTrial")]
    public class TimeTrial
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Production Number")]
        public int ProductionNumber { get; set; }

        [Display(Name = "Sales Order")]
        public int SalesOrder { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Total Time is Required")]
        [Display(Name = "Total Production Time")]
        public decimal TotalTime { get; set; }

        [Display(Name = "Drive Production Time")]
        public decimal DriveTime { get; set; }

        [Display(Name = "AV Production Time")]
        public decimal AVTime { get; set; }

        [Display(Name = "Number of Options")]
        public int NumOptions { get; set; }

        [Display(Name = "Options Text")]
        public string OptionsText { get; set; }

        public virtual ICollection<TimeTrialsOptionTime> TTOptionTimes { get; set; }

        public virtual Model Model { get; set; }
    }
}
