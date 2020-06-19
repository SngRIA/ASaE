namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEquipmentEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EquipmentServices",
                c => new
                    {
                        Equipment_Id = c.Int(nullable: false),
                        Service_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Equipment_Id, t.Service_Id })
                .ForeignKey("dbo.Equipments", t => t.Equipment_Id, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.Service_Id, cascadeDelete: true)
                .Index(t => t.Equipment_Id)
                .Index(t => t.Service_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EquipmentServices", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.EquipmentServices", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.EquipmentServices", new[] { "Service_Id" });
            DropIndex("dbo.EquipmentServices", new[] { "Equipment_Id" });
            DropTable("dbo.EquipmentServices");
        }
    }
}
