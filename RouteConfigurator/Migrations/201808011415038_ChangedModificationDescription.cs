namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedModificationDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Modification", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Modification", "Description", c => c.String(unicode: false, storeType: "text"));
        }
    }
}
