namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDb1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ChangeValueConsumables", newName: "ModifiedValueConsumables");
            RenameTable(name: "dbo.EquipmentEquipmentConsumables", newName: "EquipmentConsumableEquipments");
            DropPrimaryKey("dbo.EquipmentConsumableEquipments");
            AddColumn("dbo.ModifiedValueConsumables", "Date", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.EquipmentConsumableEquipments", new[] { "EquipmentConsumable_Id", "Equipment_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.EquipmentConsumableEquipments");
            DropColumn("dbo.ModifiedValueConsumables", "Date");
            AddPrimaryKey("dbo.EquipmentConsumableEquipments", new[] { "Equipment_Id", "EquipmentConsumable_Id" });
            RenameTable(name: "dbo.EquipmentConsumableEquipments", newName: "EquipmentEquipmentConsumables");
            RenameTable(name: "dbo.ModifiedValueConsumables", newName: "ChangeValueConsumables");
        }
    }
}
