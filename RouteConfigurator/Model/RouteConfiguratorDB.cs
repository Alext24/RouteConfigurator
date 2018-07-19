using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    public class RouteConfiguratorDB : DbContext
    {
        public RouteConfiguratorDB() : base("RouteConfiguratorDB")
        {

        }

        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<TimeTrial> TimeTrials { get; set; }
        public virtual DbSet<TimeTrialsOptionTime> TTOptionTimes { get; set; }
        public virtual DbSet<Override> Overrides { get; set; }
        public virtual DbSet<Modification> Modifications { get; set; }
    }
}
