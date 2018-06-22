﻿using System;
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
        [Key]
        public int ProductionNumber { get; set; }

        public int SalesOrder { get; set; }

        public decimal BaseTime { get; set; }

        public decimal AVTime { get; set; }

        public virtual ICollection<TimeTrialsOptionTime> TTOptionTimes { get; set; }
    }
}
