namespace OPIDChecks.DataContexts.OPIDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Invitation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Extended = c.DateTime(nullable: false),
                        Accepted = c.DateTime(nullable: false),
                        UserName = c.String(),
                        FullName = c.String(),
                        Email = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Invitations");
        }
    }
}
