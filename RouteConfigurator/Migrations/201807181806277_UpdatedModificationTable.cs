namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedModificationTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Modification", "NewModel_Base", "dbo.Model");
            DropForeignKey("dbo.Modification", new[] { "NewOption_OptionCode", "NewOption_BoxSize" }, "dbo.Option");
            DropIndex("dbo.Modification", new[] { "NewModel_Base" });
            DropIndex("dbo.Modification", new[] { "NewOption_OptionCode", "NewOption_BoxSize" });
            AddColumn("dbo.Modification", "IsOption", c => c.Boolean(nullable: false));
            AddColumn("dbo.Modification", "IsNew", c => c.Boolean(nullable: false));
            AddColumn("dbo.Modification", "NewBoxSize", c => c.String());
            AddColumn("dbo.Modification", "NewBase", c => c.String());
            AddColumn("dbo.Modification", "NewDriveTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Modification", "NewAVTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Modification", "NewTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Modification", "NewName", c => c.String());
            DropColumn("dbo.Modification", "NewModel_Base");
            DropColumn("dbo.Modification", "NewOption_OptionCode");
            DropColumn("dbo.Modification", "NewOption_BoxSize");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Modification", "NewOption_BoxSize", c => c.String(maxLength: 5));
            AddColumn("dbo.Modification", "NewOption_OptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.Modification", "NewModel_Base", c => c.String(maxLength: 8));
            DropColumn("dbo.Modification", "NewName");
            DropColumn("dbo.Modification", "NewTime");
            DropColumn("dbo.Modification", "NewAVTime");
            DropColumn("dbo.Modification", "NewDriveTime");
            DropColumn("dbo.Modification", "NewBase");
            DropColumn("dbo.Modification", "NewBoxSize");
            DropColumn("dbo.Modification", "IsNew");
            DropColumn("dbo.Modification", "IsOption");
            CreateIndex("dbo.Modification", new[] { "NewOption_OptionCode", "NewOption_BoxSize" });
            CreateIndex("dbo.Modification", "NewModel_Base");
            AddForeignKey("dbo.Modification", new[] { "NewOption_OptionCode", "NewOption_BoxSize" }, "dbo.Option", new[] { "OptionCode", "BoxSize" });
            AddForeignKey("dbo.Modification", "NewModel_Base", "dbo.Model", "Base");
        }
    }
}
