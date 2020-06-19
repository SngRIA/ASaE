namespace ASaE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServices1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Services", "Picture", c => c.Binary());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Picture", c => c.Binary(nullable: false));
        }
    }
}
