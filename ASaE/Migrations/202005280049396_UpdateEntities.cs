namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Requests", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "Date");
            DropColumn("dbo.Contracts", "Date");
        }
    }
}
