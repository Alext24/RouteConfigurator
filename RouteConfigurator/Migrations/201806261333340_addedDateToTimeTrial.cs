namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDateToTimeTrial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeTrial", "Date", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeTrial", "Date");
        }
    }
}
