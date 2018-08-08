using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    [Table("Component")]
    public class Component
    {
        [Key]
        [Column(Order = 1)]
        public string ComponentName{ get; set; }

        [Key]
        [Column(Order = 2)]
        public string EnclosureSize{ get; set; }

        [Required(ErrorMessage = "Time is Required")]
        public decimal Time { get; set; }
    }
}
