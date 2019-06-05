using OPIDChecks.Models;
using OPIDEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDChecks.Utils
{
    public class MyExcelDataReader
    {
        public static List<CheckViewModel> GetCVMS(string filePath)
        {
            List<CheckViewModel> rchecks = new ExcelData(filePath).GetData().Select(dataRow => new CheckViewModel
            {
                Date = dataRow["Date"].ToString(),
                RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                InterviewRecordID = (DBNull.Value.Equals(dataRow["Interview Record ID"]) ? 0 : Convert.ToInt32(dataRow["Interview Record ID"].ToString())),
                Name = dataRow["Name"].ToString(),
                Num = Convert.ToInt32(dataRow["Check Number"].ToString()),
                Service = dataRow["Service"].ToString(),
                Disposition = dataRow["Disposition"].ToString()
            }).ToList();

            return rchecks;
        }

    }
}