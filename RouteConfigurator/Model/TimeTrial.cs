using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("TimeTrials")]
    public class TimeTrial
    {
        [Key]
        public int ProductionNumber;

        public int SalesOrder;

        public decimal BaseTime;

        public decimal AVTime;
    }
}
