namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRouteQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RouteQueue",
                c => new
                    {
                        RouteQueueID = c.Int(nullable: false, identity: true),
                        Route = c.Int(nullable: false),
                        MaterialNumber = c.String(),
                        Line = c.String(),
                        TotalTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.RouteQueueID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RouteQueue");
        }
    }
}
