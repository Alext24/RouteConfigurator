using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("TimeTrialsOptionTime")]
    public class TimeTrialsOptionTime
    {
        [Key]
        [Column(Order=1)]
        [StringLength(2)]
        public string OptionCode { get; set; }

        [Key]
        [Column(Order=2)]
        public int ProductionNumber { get; set; }

        public decimal Time { get; set; }

        public virtual TimeTrial TimeTrial { get; set; }
    }
}
