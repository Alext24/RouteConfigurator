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
                        BoxSize = c.String(maxLength: 5),
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(maxLength: 45),
                        Model_ModelNum = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.OptionCode)
                .ForeignKey("dbo.Model", t => t.Model_ModelNum)
                .Index(t => t.Model_ModelNum);
            
            CreateTable(
                "dbo.TimeTrial",
                c => new
                    {
                        ProductionNumber = c.Int(nullable: false, identity: true),
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
                        Time = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeTrial_ProductionNumber = c.Int(),
                    })
                .PrimaryKey(t => t.OptionCode)
                .ForeignKey("dbo.TimeTrial", t => t.TimeTrial_ProductionNumber)
                .Index(t => t.TimeTrial_ProductionNumber);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeTrial", "Model_ModelNum", "dbo.Model");
            DropForeignKey("dbo.TimeTrialsOptionTime", "TimeTrial_ProductionNumber", "dbo.TimeTrial");
            DropForeignKey("dbo.Option", "Model_ModelNum", "dbo.Model");
            DropIndex("dbo.TimeTrialsOptionTime", new[] { "TimeTrial_ProductionNumber" });
            DropIndex("dbo.TimeTrial", new[] { "Model_ModelNum" });
            DropIndex("dbo.Option", new[] { "Model_ModelNum" });
            DropTable("dbo.TimeTrialsOptionTime");
            DropTable("dbo.TimeTrial");
            DropTable("dbo.Option");
            DropTable("dbo.Model");
        }
    }
}
