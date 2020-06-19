namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.Services", new[] { "Equipment_Id" });
            AlterColumn("dbo.Services", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Services", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Services", "Equipment_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Services", "Equipment_Id");
            AddForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.Services", new[] { "Equipment_Id" });
            AlterColumn("dbo.Services", "Equipment_Id", c => c.Int());
            AlterColumn("dbo.Services", "Description", c => c.String());
            AlterColumn("dbo.Services", "Name", c => c.String());
            CreateIndex("dbo.Services", "Equipment_Id");
            AddForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments", "Id");
        }
    }
}
