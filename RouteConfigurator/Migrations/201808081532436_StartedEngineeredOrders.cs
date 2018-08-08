namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StartedEngineeredOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Component",
                c => new
                    {
                        ComponentName = c.String(nullable: false, maxLength: 128),
                        EnclosureSize = c.String(nullable: false, maxLength: 128),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.ComponentName, t.EnclosureSize });
            
            CreateTable(
                "dbo.Enclosure",
                c => new
                    {
                        EnclosureType = c.String(nullable: false, maxLength: 128),
                        EnclosureSize = c.String(nullable: false, maxLength: 128),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.EnclosureType, t.EnclosureSize });
            
            CreateTable(
                "dbo.WireGauge",
                c => new
                    {
                        Gauge = c.String(nullable: false, maxLength: 128),
                        TimePercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Gauge);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WireGauge");
            DropTable("dbo.Enclosure");
            DropTable("dbo.Component");
        }
    }
}
