using System;
using System.Collections.Generic;
using System.Text;

namespace OPIDChecks.Models
{
    public class CheckViewModel
    {
        public int Id { get; set; }
        public string RecordID { get; set; }
        public string InterviewRecordID { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Date { get; set; }
        public string Service { get; set; }
        public string Disposition { get; set; }
    }
}
