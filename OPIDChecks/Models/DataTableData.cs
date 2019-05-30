using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDChecks.Models
{
    public class DataTableData
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<CheckViewModel> data { get; set; }
    }
}