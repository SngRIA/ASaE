namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDateRules : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contracts", "Date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Requests", "Date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.ModifiedValueConsumables", "Date", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ModifiedValueConsumables", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Requests", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Contracts", "Date", c => c.DateTime(nullable: false));
        }
    }
}
