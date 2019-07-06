using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDChecks.Models
{
    public class Check
    {
        public DateTime Date { get; set; }

        public int Num { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        public string Service { get; set; }

        public string Disposition { get; set; }
 
        public string Amount { get; set; }

        public int RecordID { get; set; }

        public int InterviewRecordID { get; set; }
    }
}