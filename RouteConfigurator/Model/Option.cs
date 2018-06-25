using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("Option")]
    public class Option
    {
        [Key]
        [Column(Order=1)]
        [StringLength(2)]
        public string OptionCode { get; set; }

        [StringLength(5)]
        public string BoxSize { get; set; }

        public decimal Time { get; set; }

        [StringLength(45)]
        public string Name { get; set; }

        public virtual Model Model { get; set; }

        [Key]
        [Column(Order=2)]
        public string ModelNum { get; set; }
    }
}
