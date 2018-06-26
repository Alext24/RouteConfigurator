namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedTotalTimeToTimeTrial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeTrial", "TotalTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeTrial", "TotalTime");
        }
    }
}
