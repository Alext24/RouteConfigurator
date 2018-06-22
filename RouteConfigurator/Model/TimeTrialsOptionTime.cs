using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("TimeTrialsOptionTimes")]
    public class TimeTrialsOptionTime
    {
        [Key]
        [StringLength(2)]
        public string OptionCode;

        public decimal Time;
    }
}
