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
                sRecordID = dataRow["Record ID"].ToString(),
                InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                sInterviewRecordID = dataRow["Interview Record ID"].ToString(),
                Name = dataRow["Name"].ToString(),
                Num = Convert.ToInt32(dataRow["Check Number"].ToString()),
                sNum = dataRow["Check Number"].ToString(),
                Service = dataRow["Service"].ToString(),
                Disposition = dataRow["Disposition"].ToString()
            }).ToList();

            return rchecks;
        }

        public static List<DispositionRow> GetResearchRows(string filePath)
        {
            try
            {
                List<DispositionRow> resRows = new ExcelData(filePath).GetData().Select(dataRow => new DispositionRow
                {
                    RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                    Lname = dataRow["Last Name"].ToString(),
                    Fname = dataRow["First Name"].ToString(),
                    InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                    Date = Convert.ToDateTime(dataRow["OPID Interview Date"].ToString()),
                    LBVDCheckNum = Convert.ToInt32(dataRow["LBVD Check Number"].ToString()),
                    LBVDCheckDisposition = dataRow["LBVD Check Disposition"].ToString(),
                    LBVDCheckNum2 = Convert.ToInt32(dataRow["LBVD Check Number Two"].ToString()),
                    LBVDCheck2Disposition = dataRow["LBVD Check Two Disposition"].ToString(),
                    LBVDCheckNum3 = Convert.ToInt32(dataRow["LBVD Check Number Three"].ToString()),
                    LBVDCheck3Disposition = dataRow["LBVD Check Three Disposition"].ToString(),
                    TIDCheckNum = Convert.ToInt32(dataRow["TID Check Number"].ToString()),
                    TIDCheckDisposition = dataRow["TID Check Disposition"].ToString(),
                    TIDCheckNum2 = Convert.ToInt32(dataRow["TID Check Number Two"].ToString()),
                    TIDCheck2Disposition = dataRow["TID Check Two Disposition"].ToString(),
                    TIDCheckNum3 = Convert.ToInt32(dataRow["TID Check Number Three"].ToString()),
                    TIDCheck3Disposition = dataRow["TID Check Three Disposition"].ToString(),
                    TDLCheckNum = Convert.ToInt32(dataRow["TDL Check Number"].ToString()),
                    TDLCheckDisposition = dataRow["TDL Check Disposition"].ToString(),
                    TDLCheckNum2 = Convert.ToInt32(dataRow["TDL Check Number Two"].ToString()),
                    TDLCheck2Disposition = dataRow["TDL Check Two Disposition"].ToString(),
                    TDLCheckNum3 = Convert.ToInt32(dataRow["TDL Check Number Three"].ToString()),
                    TDLCheck3Disposition = dataRow["TDL Check Three Disposition"].ToString(),
                    MBVDCheckNum = Convert.ToInt32(dataRow["MBVD Check Number"].ToString()),
                    MBVDCheckDisposition = dataRow["MBVD Check Disposition"].ToString(),
                    MBVDCheckNum2 = Convert.ToInt32(dataRow["MBVD Check Number Two"].ToString()),
                    MBVDCheck2Disposition = dataRow["MBVD Check Two Disposition"].ToString(),
                    MBVDCheckNum3 = Convert.ToInt32(dataRow["MBVD Check Number Three"].ToString()),
                    MBVDCheck3Disposition = dataRow["MBVD Check Three Disposition"].ToString(),
                    //    SDCheckNum = Convert.ToInt32(dataRow["SD Check Number"].ToString()),
                    //    SDCheckDisposition = dataRow["SD Check Disposition"].ToString()
                }).ToList();

                return resRows;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}