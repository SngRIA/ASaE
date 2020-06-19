namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "UserComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "UserComment");
        }
    }
}
