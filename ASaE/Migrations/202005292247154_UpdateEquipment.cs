namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEquipment : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Equipments", "Cost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Equipments", "Cost", c => c.Int(nullable: false));
        }
    }
}
