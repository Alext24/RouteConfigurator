namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedModificationTable2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Modification", "OldModel_Base", "dbo.Model");
            DropForeignKey("dbo.Modification", new[] { "OldOption_OptionCode", "OldOption_BoxSize" }, "dbo.Option");
            DropIndex("dbo.Modification", new[] { "OldModel_Base" });
            DropIndex("dbo.Modification", new[] { "OldOption_OptionCode", "OldOption_BoxSize" });
            AddColumn("dbo.Modification", "OldModelBase", c => c.String(maxLength: 8));
            AddColumn("dbo.Modification", "OldModelDriveTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Modification", "OldModelAVTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Modification", "NewOptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.Modification", "OldOptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.Modification", "OldOptionTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Modification", "OldOptionName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Modification", "NewBoxSize", c => c.String(maxLength: 5));
            AlterColumn("dbo.Modification", "NewBase", c => c.String(maxLength: 8));
            AlterColumn("dbo.Modification", "NewName", c => c.String(maxLength: 100));
            DropColumn("dbo.Modification", "OldModel_Base");
            DropColumn("dbo.Modification", "OldOption_OptionCode");
            DropColumn("dbo.Modification", "OldOption_BoxSize");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Modification", "OldOption_BoxSize", c => c.String(maxLength: 5));
            AddColumn("dbo.Modification", "OldOption_OptionCode", c => c.String(maxLength: 2));
            AddColumn("dbo.Modification", "OldModel_Base", c => c.String(maxLength: 8));
            AlterColumn("dbo.Modification", "NewName", c => c.String());
            AlterColumn("dbo.Modification", "NewBase", c => c.String());
            AlterColumn("dbo.Modification", "NewBoxSize", c => c.String());
            DropColumn("dbo.Modification", "OldOptionName");
            DropColumn("dbo.Modification", "OldOptionTime");
            DropColumn("dbo.Modification", "OldOptionCode");
            DropColumn("dbo.Modification", "NewOptionCode");
            DropColumn("dbo.Modification", "OldModelAVTime");
            DropColumn("dbo.Modification", "OldModelDriveTime");
            DropColumn("dbo.Modification", "OldModelBase");
            CreateIndex("dbo.Modification", new[] { "OldOption_OptionCode", "OldOption_BoxSize" });
            CreateIndex("dbo.Modification", "OldModel_Base");
            AddForeignKey("dbo.Modification", new[] { "OldOption_OptionCode", "OldOption_BoxSize" }, "dbo.Option", new[] { "OptionCode", "BoxSize" });
            AddForeignKey("dbo.Modification", "OldModel_Base", "dbo.Model", "Base");
        }
    }
}
