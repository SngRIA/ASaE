namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConsumable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Equipments", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Equipments", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.EquipmentConsumables", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.EquipmentConsumables", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EquipmentConsumables", "Description", c => c.String());
            AlterColumn("dbo.EquipmentConsumables", "Name", c => c.String());
            AlterColumn("dbo.Equipments", "Description", c => c.String());
            AlterColumn("dbo.Equipments", "Name", c => c.String());
        }
    }
}
