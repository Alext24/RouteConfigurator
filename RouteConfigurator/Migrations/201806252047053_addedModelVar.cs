namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedModelVar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Model", "ExtraTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Model", "ExtraTime");
        }
    }
}
