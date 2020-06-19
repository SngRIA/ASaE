namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ObjectItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Count = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        File_Id = c.Int(),
                        Service_Id = c.Int(),
                        Request_Id = c.Int(),
                        UserBasket_Id = c.Int(),
                        Contract_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileItems", t => t.File_Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .ForeignKey("dbo.Requests", t => t.Request_Id)
                .ForeignKey("dbo.UserBaskets", t => t.UserBasket_Id)
                .ForeignKey("dbo.Contracts", t => t.Contract_Id)
                .Index(t => t.File_Id)
                .Index(t => t.Service_Id)
                .Index(t => t.Request_Id)
                .Index(t => t.UserBasket_Id)
                .Index(t => t.Contract_Id);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Service_Id = c.Int(),
                        Client_Id = c.String(maxLength: 128),
                        Employee_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Client_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Employee_Id)
                .Index(t => t.Service_Id)
                .Index(t => t.Client_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserBaskets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Service_Id = c.Int(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Service_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.FileItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PathFile = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Cost = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Client_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.EquipmentConsumables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Cost = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        EquipmentConsumable_Id = c.Int(),
                        EquipmentManager_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentConsumables", t => t.EquipmentConsumable_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.EquipmentManager_Id)
                .Index(t => t.EquipmentConsumable_Id)
                .Index(t => t.EquipmentManager_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Equipments", "EquipmentManager_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Equipments", "EquipmentConsumable_Id", "dbo.EquipmentConsumables");
            DropForeignKey("dbo.Employees", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ObjectItems", "Contract_Id", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "Employee_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Contracts", "Client_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserBaskets", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ObjectItems", "UserBasket_Id", "dbo.UserBaskets");
            DropForeignKey("dbo.ObjectItems", "Request_Id", "dbo.Requests");
            DropForeignKey("dbo.Requests", "Client_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ObjectItems", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.UserBaskets", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Contracts", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.ObjectItems", "File_Id", "dbo.FileItems");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Equipments", new[] { "EquipmentManager_Id" });
            DropIndex("dbo.Equipments", new[] { "EquipmentConsumable_Id" });
            DropIndex("dbo.Employees", new[] { "User_Id" });
            DropIndex("dbo.Requests", new[] { "Client_Id" });
            DropIndex("dbo.UserBaskets", new[] { "User_Id" });
            DropIndex("dbo.UserBaskets", new[] { "Service_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Contracts", new[] { "Employee_Id" });
            DropIndex("dbo.Contracts", new[] { "Client_Id" });
            DropIndex("dbo.Contracts", new[] { "Service_Id" });
            DropIndex("dbo.ObjectItems", new[] { "Contract_Id" });
            DropIndex("dbo.ObjectItems", new[] { "UserBasket_Id" });
            DropIndex("dbo.ObjectItems", new[] { "Request_Id" });
            DropIndex("dbo.ObjectItems", new[] { "Service_Id" });
            DropIndex("dbo.ObjectItems", new[] { "File_Id" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Equipments");
            DropTable("dbo.EquipmentConsumables");
            DropTable("dbo.Employees");
            DropTable("dbo.Requests");
            DropTable("dbo.Services");
            DropTable("dbo.FileItems");
            DropTable("dbo.UserBaskets");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Contracts");
            DropTable("dbo.ObjectItems");
        }
    }
}
