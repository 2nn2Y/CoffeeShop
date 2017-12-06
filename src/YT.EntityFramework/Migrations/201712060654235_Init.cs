namespace YT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreOrder", "Reson", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreOrder", "Reson");
        }
    }
}
