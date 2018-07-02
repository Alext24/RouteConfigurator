namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumOptionsInTimeTrial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeTrial", "NumOptions", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeTrial", "NumOptions");
        }
    }
}
