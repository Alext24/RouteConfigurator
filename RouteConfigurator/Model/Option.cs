using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("Options")]
    public class Option
    {
        [Key]
        [StringLength(2)]
        public string OptionCode;

        [StringLength(5)]
        public string BoxSize;

        public decimal Time;

        [StringLength(45)]
        public string Name;
    }
}
