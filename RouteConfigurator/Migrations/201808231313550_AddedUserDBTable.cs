namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserDBTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 15),
                        LastName = c.String(nullable: false, maxLength: 20),
                        PasswordHash = c.String(nullable: false, maxLength: 64),
                        Salt = c.Binary(nullable: false, maxLength: 256),
                        EmployeeType = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Email);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
