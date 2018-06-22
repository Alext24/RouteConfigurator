using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("Models")]
    public class Model
    {
        [Key]
        [StringLength(30)]
        public string ModelNum;

        public int RouteNum;

        public decimal TotalTime;

        public bool IsOverrideActive;

        public int OverrideRoute;

        public decimal OverrideTime;

        [StringLength(5)]
        public string BoxSize;

        public decimal BaseTime;

        public decimal AVTime;
    }
}
