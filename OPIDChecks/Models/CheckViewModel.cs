using System;
using System.Collections.Generic;
using System.Text;

namespace OPIDChecks.Models
{
    public class CheckViewModel
    {
        public int Id { get; set; }
        public int RecordID { get; set; }
        public int InterviewRecordID { get; set; }
        public string Name { get; set; }
        public int Num { get; set; }
        public string Date { get; set; }
        public string Service { get; set; }
        public string Disposition { get; set; }
    }
}
