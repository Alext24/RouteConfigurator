namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Model",
                c => new
                    {
                        ModelNum = c.String(nullable: false, maxLength: 30),
                        RouteNum = c.Int(nullable: false),
                        TotalTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsOverrideActive = c.Boolean(nullable: false),
                        OverrideRoute = c.Int(nullable: false),
                        OverrideTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BoxSize = c.String(maxLength: 5),
                        BaseTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AVTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ModelNum);
            
            CreateTable(
                "dbo.Option",
                c => new
                    {
                        OptionCode = c.String(nullable: false, maxLength: 2),
                        ModelNum = c.String(nullable: false, maxLength: 30),
                        BoxSize = c.String(maxLength: 5),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(maxLength: 45),
                    })
                .PrimaryKey(t => new { t.OptionCode, t.ModelNum })
                .ForeignKey("dbo.Model", t => t.ModelNum, cascadeDelete: true)
                .Index(t => t.ModelNum);
            
            CreateTable(
                "dbo.TimeTrial",
                c => new
                    {
                        ProductionNumber = c.Int(nullable: false),
                        SalesOrder = c.Int(nullable: false),
                        BaseTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AVTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Model_ModelNum = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ProductionNumber)
                .ForeignKey("dbo.Model", t => t.Model_ModelNum)
                .Index(t => t.Model_ModelNum);
            
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
            DropForeignKey("dbo.TimeTrial", "Model_ModelNum", "dbo.Model");
            DropForeignKey("dbo.Option", "ModelNum", "dbo.Model");
            DropIndex("dbo.TimeTrialsOptionTime", new[] { "ProductionNumber" });
            DropIndex("dbo.TimeTrial", new[] { "Model_ModelNum" });
            DropIndex("dbo.Option", new[] { "ModelNum" });
            DropTable("dbo.TimeTrialsOptionTime");
            DropTable("dbo.TimeTrial");
            DropTable("dbo.Option");
            DropTable("dbo.Model");
        }
    }
}
