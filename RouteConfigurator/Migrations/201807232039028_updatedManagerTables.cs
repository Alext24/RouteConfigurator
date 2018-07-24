namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedManagerTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Modification", "ReviewDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Modification", "BoxSize", c => c.String(maxLength: 5));
            AddColumn("dbo.Modification", "ModelBase", c => c.String(maxLength: 8));
            AddColumn("dbo.Modification", "OptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.OverrideRequest", "ReviewDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.Modification", "NewBoxSize");
            DropColumn("dbo.Modification", "NewBase");
            DropColumn("dbo.Modification", "OldModelBase");
            DropColumn("dbo.Modification", "NewOptionCode");
            DropColumn("dbo.Modification", "OldOptionCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Modification", "OldOptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.Modification", "NewOptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.Modification", "OldModelBase", c => c.String(maxLength: 8));
            AddColumn("dbo.Modification", "NewBase", c => c.String(maxLength: 8));
            AddColumn("dbo.Modification", "NewBoxSize", c => c.String(maxLength: 5));
            DropColumn("dbo.OverrideRequest", "ReviewDate");
            DropColumn("dbo.Modification", "OptionCode");
            DropColumn("dbo.Modification", "ModelBase");
            DropColumn("dbo.Modification", "BoxSize");
            DropColumn("dbo.Modification", "ReviewDate");
        }
    }
}
