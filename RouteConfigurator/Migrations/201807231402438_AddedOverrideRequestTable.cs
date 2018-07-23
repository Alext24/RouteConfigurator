namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOverrideRequestTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OverrideRequest",
                c => new
                    {
                        OverrideRequestID = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false, storeType: "date"),
                        Description = c.String(unicode: false, storeType: "text"),
                        State = c.Int(nullable: false),
                        Sender = c.String(nullable: false),
                        Reviewer = c.String(),
                        ModelNum = c.String(maxLength: 64),
                        OverrideTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverrideRoute = c.Int(nullable: false),
                        ModelTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ModelRoute = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OverrideRequestID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OverrideRequest");
        }
    }
}
