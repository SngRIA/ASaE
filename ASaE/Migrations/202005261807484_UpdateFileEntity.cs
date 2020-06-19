namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFileEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileItems", "Size", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileItems", "Size");
        }
    }
}
