namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModelBasePropertyToOverrideRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OverrideRequest", "ModelBase", c => c.String(nullable: false, maxLength: 8));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OverrideRequest", "ModelBase");
        }
    }
}
