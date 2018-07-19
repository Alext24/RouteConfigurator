namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModificationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Modification",
                c => new
                    {
                        ModificationID = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false, storeType: "date"),
                        Description = c.String(unicode: false, storeType: "text"),
                        State = c.Int(nullable: false),
                        Reviewer = c.String(),
                        Sender = c.String(nullable: false),
                        NewModel_Base = c.String(maxLength: 8),
                        NewOption_OptionCode = c.String(maxLength: 2),
                        NewOption_BoxSize = c.String(maxLength: 5),
                        OldModel_Base = c.String(maxLength: 8),
                        OldOption_OptionCode = c.String(maxLength: 2),
                        OldOption_BoxSize = c.String(maxLength: 5),
                    })
                .PrimaryKey(t => t.ModificationID)
                .ForeignKey("dbo.Model", t => t.NewModel_Base)
                .ForeignKey("dbo.Option", t => new { t.NewOption_OptionCode, t.NewOption_BoxSize })
                .ForeignKey("dbo.Model", t => t.OldModel_Base)
                .ForeignKey("dbo.Option", t => new { t.OldOption_OptionCode, t.OldOption_BoxSize })
                .Index(t => t.NewModel_Base)
                .Index(t => new { t.NewOption_OptionCode, t.NewOption_BoxSize })
                .Index(t => t.OldModel_Base)
                .Index(t => new { t.OldOption_OptionCode, t.OldOption_BoxSize });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Modification", new[] { "OldOption_OptionCode", "OldOption_BoxSize" }, "dbo.Option");
            DropForeignKey("dbo.Modification", "OldModel_Base", "dbo.Model");
            DropForeignKey("dbo.Modification", new[] { "NewOption_OptionCode", "NewOption_BoxSize" }, "dbo.Option");
            DropForeignKey("dbo.Modification", "NewModel_Base", "dbo.Model");
            DropIndex("dbo.Modification", new[] { "OldOption_OptionCode", "OldOption_BoxSize" });
            DropIndex("dbo.Modification", new[] { "OldModel_Base" });
            DropIndex("dbo.Modification", new[] { "NewOption_OptionCode", "NewOption_BoxSize" });
            DropIndex("dbo.Modification", new[] { "NewModel_Base" });
            DropTable("dbo.Modification");
        }
    }
}
