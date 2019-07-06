namespace OPIDChecks.DataContexts.OPIDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Matched : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RChecks", "Matched");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RChecks", "Matched", c => c.Boolean(nullable: false));
        }
    }
}
