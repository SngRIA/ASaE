namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServicesRules : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.Services", new[] { "Equipment_Id" });
            AlterColumn("dbo.Services", "Equipment_Id", c => c.Int());
            CreateIndex("dbo.Services", "Equipment_Id");
            AddForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.Services", new[] { "Equipment_Id" });
            AlterColumn("dbo.Services", "Equipment_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Services", "Equipment_Id");
            AddForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments", "Id", cascadeDelete: true);
        }
    }
}
