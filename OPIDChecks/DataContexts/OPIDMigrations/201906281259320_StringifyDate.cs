namespace OPIDChecks.DataContexts.OPIDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StringifyDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RChecks", "sDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RChecks", "sDate");
        }
    }
}
