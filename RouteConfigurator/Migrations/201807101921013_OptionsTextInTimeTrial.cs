namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OptionsTextInTimeTrial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeTrial", "OptionsText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeTrial", "OptionsText");
        }
    }
}
