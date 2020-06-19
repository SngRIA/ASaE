namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServiceEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EquipmentServices", "Equipment_Id", "dbo.Equipments");
            DropForeignKey("dbo.EquipmentServices", "Service_Id", "dbo.Services");
            DropIndex("dbo.EquipmentServices", new[] { "Equipment_Id" });
            DropIndex("dbo.EquipmentServices", new[] { "Service_Id" });
            AddColumn("dbo.Services", "Equipment_Id", c => c.Int());
            CreateIndex("dbo.Services", "Equipment_Id");
            AddForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments", "Id");
            DropTable("dbo.EquipmentServices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EquipmentServices",
                c => new
                    {
                        Equipment_Id = c.Int(nullable: false),
                        Service_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Equipment_Id, t.Service_Id });
            
            DropForeignKey("dbo.Services", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.Services", new[] { "Equipment_Id" });
            DropColumn("dbo.Services", "Equipment_Id");
            CreateIndex("dbo.EquipmentServices", "Service_Id");
            CreateIndex("dbo.EquipmentServices", "Equipment_Id");
            AddForeignKey("dbo.EquipmentServices", "Service_Id", "dbo.Services", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EquipmentServices", "Equipment_Id", "dbo.Equipments", "Id", cascadeDelete: true);
        }
    }
}
