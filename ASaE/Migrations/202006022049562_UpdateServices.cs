namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Picture", c => c.Binary(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Picture");
        }
    }
}
