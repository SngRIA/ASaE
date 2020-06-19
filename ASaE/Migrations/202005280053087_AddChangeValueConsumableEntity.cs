namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangeValueConsumableEntity : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EquipmentConsumableEquipments", newName: "EquipmentEquipmentConsumables");
            DropPrimaryKey("dbo.EquipmentEquipmentConsumables");
            CreateTable(
                "dbo.ChangeValueConsumables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldValue = c.Int(nullable: false),
                        NewValue = c.Int(nullable: false),
                        EquipmentConsumable_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentConsumables", t => t.EquipmentConsumable_Id)
                .Index(t => t.EquipmentConsumable_Id);
            
            AddPrimaryKey("dbo.EquipmentEquipmentConsumables", new[] { "Equipment_Id", "EquipmentConsumable_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeValueConsumables", "EquipmentConsumable_Id", "dbo.EquipmentConsumables");
            DropIndex("dbo.ChangeValueConsumables", new[] { "EquipmentConsumable_Id" });
            DropPrimaryKey("dbo.EquipmentEquipmentConsumables");
            DropTable("dbo.ChangeValueConsumables");
            AddPrimaryKey("dbo.EquipmentEquipmentConsumables", new[] { "EquipmentConsumable_Id", "Equipment_Id" });
            RenameTable(name: "dbo.EquipmentEquipmentConsumables", newName: "EquipmentConsumableEquipments");
        }
    }
}
