namespace OPIDChecks.DataContexts.OPIDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RCheckStrings : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RChecks", "RecordID", c => c.String());
            AlterColumn("dbo.RChecks", "InterviewRecordID", c => c.String());
            AlterColumn("dbo.RChecks", "Num", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RChecks", "Num", c => c.Int(nullable: false));
            AlterColumn("dbo.RChecks", "InterviewRecordID", c => c.Int(nullable: false));
            AlterColumn("dbo.RChecks", "RecordID", c => c.Int(nullable: false));
        }
    }
}
