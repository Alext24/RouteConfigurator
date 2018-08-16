namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedProductLineToModification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Modification", "ProductLine", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Modification", "ProductLine");
        }
    }
}
