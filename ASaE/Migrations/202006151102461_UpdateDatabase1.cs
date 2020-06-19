namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "UserComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contracts", "UserComment");
        }
    }
}
