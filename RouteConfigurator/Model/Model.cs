using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("Model")]
    public class Model
    {
        [Key]
        [StringLength(30)]
        public string ModelNum { get; set; }

        public int RouteNum { get; set; }

        public decimal TotalTime { get; set; }

        public bool IsOverrideActive { get; set; }

        public int OverrideRoute { get; set; }

        public decimal OverrideTime { get; set; }

        [StringLength(5)]
        public string BoxSize { get; set; }

        public decimal BaseTime { get; set; }

        public decimal AVTime { get; set; }

        public decimal ExtraTime { get; set; }

        public virtual ICollection<Option> Options { get; set; }

        public virtual ICollection<TimeTrial> TimeTrials { get; set; }
    }
}
