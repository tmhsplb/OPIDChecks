using System;
using System.Collections.Generic;
using System.Text;

namespace OPIDEntities
{
    public class RCheck
    {
        public int Id { get; set; }
        public string RecordID { get; set; }
        public string InterviewRecordID { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public System.DateTime Date { get; set; }
        public string Service { get; set; }
        public string Disposition { get; set; }
        public bool Matched { get; set; }
    }
}
