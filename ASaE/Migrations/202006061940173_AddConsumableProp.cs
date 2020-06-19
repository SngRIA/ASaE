namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConsumableProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EquipmentConsumables", "MaxValue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EquipmentConsumables", "MaxValue");
        }
    }
}
