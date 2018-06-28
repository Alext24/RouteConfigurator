namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Model",
                c => new
                    {
                        Base = c.String(nullable: false, maxLength: 30),
                        BoxSize = c.String(maxLength: 5),
                        DriveTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AVTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtraTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Base);
            
            CreateTable(
                "dbo.Option",
                c => new
                    {
                        OptionCode = c.String(nullable: false, maxLength: 2),
                        BoxSize = c.String(nullable: false, maxLength: 5),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(maxLength: 45),
                        Model_Base = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => new { t.OptionCode, t.BoxSize })
                .ForeignKey("dbo.Model", t => t.Model_Base)
                .Index(t => t.Model_Base);
            
            CreateTable(
                "dbo.Override",
                c => new
                    {
                        ModelNum = c.String(nullable: false, maxLength: 30),
                        IsOverrideActive = c.Boolean(nullable: false),
                        OverrideRoute = c.Int(nullable: false),
                        OverrideTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Model_Base = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ModelNum)
                .ForeignKey("dbo.Model", t => t.Model_Base)
                .Index(t => t.Model_Base);
            
            CreateTable(
                "dbo.TimeTrial",
                c => new
                    {
                        ProductionNumber = c.Int(nullable: false),
                        SalesOrder = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        TotalTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DriveTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AVTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Model_Base = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ProductionNumber)
                .ForeignKey("dbo.Model", t => t.Model_Base)
                .Index(t => t.Model_Base);
            
            CreateTable(
                "dbo.TimeTrialsOptionTime",
                c => new
                    {
                        OptionCode = c.String(nullable: false, maxLength: 2),
                        ProductionNumber = c.Int(nullable: false),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.OptionCode, t.ProductionNumber })
                .ForeignKey("dbo.TimeTrial", t => t.ProductionNumber, cascadeDelete: true)
                .Index(t => t.ProductionNumber);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeTrialsOptionTime", "ProductionNumber", "dbo.TimeTrial");
            DropForeignKey("dbo.TimeTrial", "Model_Base", "dbo.Model");
            DropForeignKey("dbo.Override", "Model_Base", "dbo.Model");
            DropForeignKey("dbo.Option", "Model_Base", "dbo.Model");
            DropIndex("dbo.TimeTrialsOptionTime", new[] { "ProductionNumber" });
            DropIndex("dbo.TimeTrial", new[] { "Model_Base" });
            DropIndex("dbo.Override", new[] { "Model_Base" });
            DropIndex("dbo.Option", new[] { "Model_Base" });
            DropTable("dbo.TimeTrialsOptionTime");
            DropTable("dbo.TimeTrial");
            DropTable("dbo.Override");
            DropTable("dbo.Option");
            DropTable("dbo.Model");
        }
    }
}
