using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Model.EF_StandardModels;
using System.Data.Entity;

namespace RouteConfigurator.Model.Context
{
    public class RouteConfiguratorDB : DbContext
    {
        public RouteConfiguratorDB() : base("name=RouteConfiguratorConnectionString")
        {

        }

        public virtual DbSet<EF_StandardModels.StandardModel> Models { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<TimeTrial> TimeTrials { get; set; }
        public virtual DbSet<TimeTrialsOptionTime> TTOptionTimes { get; set; }
        public virtual DbSet<Override> Overrides { get; set; }
        public virtual DbSet<Modification> Modifications { get; set; }
        public virtual DbSet<OverrideRequest> OverrideRequests { get; set; }

        public virtual DbSet<Enclosure> Enclosures { get; set; }
        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<WireGauge> WireGauges { get; set; }
        public virtual DbSet<EngineeredModification> EngineeredModifications { get; set; }

        public virtual DbSet<RouteQueue> RouteQueues { get; set; }
    }
}
