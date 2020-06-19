namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEquipmentEntity1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Equipments", "EquipmentConsumable_Id", "dbo.EquipmentConsumables");
            DropIndex("dbo.Equipments", new[] { "EquipmentConsumable_Id" });
            CreateTable(
                "dbo.EquipmentConsumableEquipments",
                c => new
                    {
                        EquipmentConsumable_Id = c.Int(nullable: false),
                        Equipment_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EquipmentConsumable_Id, t.Equipment_Id })
                .ForeignKey("dbo.EquipmentConsumables", t => t.EquipmentConsumable_Id, cascadeDelete: true)
                .ForeignKey("dbo.Equipments", t => t.Equipment_Id, cascadeDelete: true)
                .Index(t => t.EquipmentConsumable_Id)
                .Index(t => t.Equipment_Id);
            
            DropColumn("dbo.Equipments", "EquipmentConsumable_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Equipments", "EquipmentConsumable_Id", c => c.Int());
            DropForeignKey("dbo.EquipmentConsumableEquipments", "Equipment_Id", "dbo.Equipments");
            DropForeignKey("dbo.EquipmentConsumableEquipments", "EquipmentConsumable_Id", "dbo.EquipmentConsumables");
            DropIndex("dbo.EquipmentConsumableEquipments", new[] { "Equipment_Id" });
            DropIndex("dbo.EquipmentConsumableEquipments", new[] { "EquipmentConsumable_Id" });
            DropTable("dbo.EquipmentConsumableEquipments");
            CreateIndex("dbo.Equipments", "EquipmentConsumable_Id");
            AddForeignKey("dbo.Equipments", "EquipmentConsumable_Id", "dbo.EquipmentConsumables", "Id");
        }
    }
}
