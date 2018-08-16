namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAttributesToRouteQueue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RouteQueue", "ModelNumber", c => c.String());
            AddColumn("dbo.RouteQueue", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("dbo.RouteQueue", "AddedDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.RouteQueue", "SubmittedDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.RouteQueue", "MaterialNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RouteQueue", "MaterialNumber", c => c.String());
            DropColumn("dbo.RouteQueue", "SubmittedDate");
            DropColumn("dbo.RouteQueue", "AddedDate");
            DropColumn("dbo.RouteQueue", "IsApproved");
            DropColumn("dbo.RouteQueue", "ModelNumber");
        }
    }
}
