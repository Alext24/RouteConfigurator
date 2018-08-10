namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedEngineeredModifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EngineeredModification",
                c => new
                    {
                        ModificationID = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false, storeType: "date"),
                        ReviewedDate = c.DateTime(nullable: false, storeType: "date"),
                        Description = c.String(),
                        State = c.Int(nullable: false),
                        Sender = c.String(nullable: false),
                        Reviewer = c.String(),
                        IsNew = c.Boolean(nullable: false),
                        ComponentName = c.String(),
                        EnclosureSize = c.String(),
                        EnclosureType = c.String(),
                        NewTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OldTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gauge = c.String(),
                        NewTimePercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OldTimePercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ModificationID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EngineeredModification");
        }
    }
}
