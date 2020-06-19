namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDateRules1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contracts", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Requests", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ModifiedValueConsumables", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ModifiedValueConsumables", "Date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Requests", "Date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Contracts", "Date", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
