namespace Bloggle.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ViewColumnAddedToBlogsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "Views", c => c.Int(nullable: false,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blogs", "Views");
        }
    }
}
