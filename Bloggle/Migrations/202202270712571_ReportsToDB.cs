namespace Bloggle.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportsToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContentType = c.String(),
                        ContentId = c.Int(nullable: false),
                        ReportType = c.String(),
                        ReportedCount = c.Int(defaultValue:0),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reports");
        }
    }
}
