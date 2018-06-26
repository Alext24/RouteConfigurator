namespace RouteConfigurator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedOptionsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Option", "ModelNum", "dbo.Model");
            DropIndex("dbo.Option", new[] { "ModelNum" });
            RenameColumn(table: "dbo.Option", name: "ModelNum", newName: "Model_ModelNum");
            DropPrimaryKey("dbo.Option");
            AlterColumn("dbo.Option", "Model_ModelNum", c => c.String(maxLength: 30));
            AlterColumn("dbo.Option", "BoxSize", c => c.String(nullable: false, maxLength: 5));
            AddPrimaryKey("dbo.Option", new[] { "OptionCode", "BoxSize" });
            CreateIndex("dbo.Option", "Model_ModelNum");
            AddForeignKey("dbo.Option", "Model_ModelNum", "dbo.Model", "ModelNum");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Option", "Model_ModelNum", "dbo.Model");
            DropIndex("dbo.Option", new[] { "Model_ModelNum" });
            DropPrimaryKey("dbo.Option");
            AlterColumn("dbo.Option", "BoxSize", c => c.String(maxLength: 5));
            AlterColumn("dbo.Option", "Model_ModelNum", c => c.String(nullable: false, maxLength: 30));
            AddPrimaryKey("dbo.Option", new[] { "OptionCode", "ModelNum" });
            RenameColumn(table: "dbo.Option", name: "Model_ModelNum", newName: "ModelNum");
            CreateIndex("dbo.Option", "ModelNum");
            AddForeignKey("dbo.Option", "ModelNum", "dbo.Model", "ModelNum", cascadeDelete: true);
        }
    }
}
