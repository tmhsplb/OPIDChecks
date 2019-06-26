namespace OPIDChecks.DataContexts.OPIDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RCheckExpansion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RChecks", "sRecordID", c => c.String());
            AddColumn("dbo.RChecks", "sInterviewRecordID", c => c.String());
            AddColumn("dbo.RChecks", "sNum", c => c.String());
            AlterColumn("dbo.RChecks", "RecordID", c => c.Int(nullable: false));
            AlterColumn("dbo.RChecks", "InterviewRecordID", c => c.Int(nullable: false));
            AlterColumn("dbo.RChecks", "Num", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RChecks", "Num", c => c.String());
            AlterColumn("dbo.RChecks", "InterviewRecordID", c => c.String());
            AlterColumn("dbo.RChecks", "RecordID", c => c.String());
            DropColumn("dbo.RChecks", "sNum");
            DropColumn("dbo.RChecks", "sInterviewRecordID");
            DropColumn("dbo.RChecks", "sRecordID");
        }
    }
}
