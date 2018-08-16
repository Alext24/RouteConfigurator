namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLineToModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Model", "Line", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Model", "Line");
        }
    }
}
