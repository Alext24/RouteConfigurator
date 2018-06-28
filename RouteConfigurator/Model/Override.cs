using System;
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
        [StringLength(30)]
        public string ModelNum { get; set; }

        public bool IsOverrideActive { get; set; }

        public int OverrideRoute { get; set; }

        public decimal OverrideTime { get; set; }
        
        public virtual Model Model { get; set; }
    }
}
