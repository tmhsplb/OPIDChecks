using Microsoft.AspNet.Identity.EntityFramework;
using OPIDChecks.DataContexts;
using OPIDChecks.Models;
using OPIDChecks.Utils;
using OPIDEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Mvc;
using DataTables.Mvc;

namespace OPIDChecks.DAL
{
    public class DataManager
    {
        private static bool firstCall = true;
        private static List<int> incidentals;

        private static List<Check> unmatchedChecks;
        private static List<Check> resolvedChecks;
        private static List<int> mistakenlyResolved;
        private static List<Check> typoChecks;

        public static void Init()
        {
            if (firstCall)
            {
                typoChecks = new List<Check>();
                resolvedChecks = new List<Check>();
                mistakenlyResolved = new List<int>();
                firstCall = false;
            }

            unmatchedChecks = new List<Check>();
            incidentals = new List<int>();
        }

        public static void PersistUnmatchedChecks(List<DispositionRow> researchRows)
        {
            List<Check> unmatchedChecks = DetermineUnmatchedChecks(researchRows);
            AppendToUnresolvedChecks(unmatchedChecks);
        }

        public static void NewUnmatchedCheck(DispositionRow row, string service, string disposition)
        {
            int checkNum;

            switch (service)
            {
                case "LBVD":
                    checkNum = row.LBVDCheckNum;
                    break;
                case "LBVD2":
                    checkNum = row.LBVDCheckNum2;
                    break;
                case "LBVD3":
                    checkNum = row.LBVDCheckNum3;
                    break;
                case "TID":
                    checkNum = row.TIDCheckNum;
                    break;
                case "TID2":
                    checkNum = row.TIDCheckNum2;
                    break;
                case "TID3":
                    checkNum = row.TIDCheckNum3;
                    break;
                case "TDL":
                    checkNum = row.TDLCheckNum;
                    break;
                case "TDL2":
                    checkNum = row.TDLCheckNum2;
                    break;
                case "TDL3":
                    checkNum = row.TDLCheckNum3;
                    break;
                case "MBVD":
                    checkNum = row.MBVDCheckNum;
                    break;
                case "MBVD2":
                    checkNum = row.MBVDCheckNum2;
                    break;
                case "MBVD3":
                    checkNum = row.MBVDCheckNum3;
                    break;
                case "SD":
                    checkNum = row.SDCheckNum;
                    break;
                default:
                    checkNum = -1;
                    break;
            }

            unmatchedChecks.Add(new Check
            {
                RecordID = row.RecordID,
                InterviewRecordID = row.InterviewRecordID,
                Num = checkNum,
                Name = string.Format("{0}, {1}", row.Lname, row.Fname),
                Date = row.Date,
                Service = service,
                Disposition = disposition
            });
        }

        private static List<Check> DetermineUnmatchedChecks(List<DispositionRow> researchRows)
        {
            foreach (DispositionRow row in researchRows)
            {
                if (row.LBVDCheckNum != 0)
                {
                    NewUnmatchedCheck(row, "LBVD", row.LBVDCheckDisposition);
                }

                if (row.LBVDCheckNum2 != 0)
                {
                    NewUnmatchedCheck(row, "LBVD2", row.LBVDCheck2Disposition);
                }

                if (row.LBVDCheckNum3 != 0)
                {
                    NewUnmatchedCheck(row, "LBVD3", row.LBVDCheck3Disposition);
                }

                if (row.TIDCheckNum != 0)
                {
                    if (!string.IsNullOrEmpty(row.TIDCheckDisposition))
                    {
                        int z;
                        z = 2;
                    }
                    NewUnmatchedCheck(row, "TID", row.TIDCheckDisposition);
                }

                if (row.TIDCheckNum2 != 0)
                {
                    NewUnmatchedCheck(row, "TID2", row.TIDCheck2Disposition);
                }

                if (row.TIDCheckNum3 != 0)
                {
                    NewUnmatchedCheck(row, "TID3", row.TIDCheck3Disposition);
                }

                if (row.TDLCheckNum != 0)
                {
                    NewUnmatchedCheck(row, "TDL", row.TDLCheckDisposition);
                }

                if (row.TDLCheckNum2 != 0)
                {
                    NewUnmatchedCheck(row, "TDL2", row.TDLCheck2Disposition);
                }

                if (row.TDLCheckNum3 != 0)
                {
                    NewUnmatchedCheck(row, "TDL3", row.TDLCheck3Disposition);
                }

                if (row.MBVDCheckNum != 0)
                {
                    NewUnmatchedCheck(row, "MBVD", row.MBVDCheckDisposition);
                }

                if (row.MBVDCheckNum2 != 0)
                {
                    NewUnmatchedCheck(row, "MBVD2", row.MBVDCheck2Disposition);
                }

                if (row.MBVDCheckNum3 != 0)
                {
                    NewUnmatchedCheck(row, "MBVD3", row.MBVDCheck3Disposition);
                }

                if (row.SDCheckNum != 0)
                {
                    NewUnmatchedCheck(row, "SD", row.SDCheckDisposition);
                }
            }

            return unmatchedChecks;
        }


        private static void AppendToUnresolvedChecks(List<Check> checks)
        {
            try
            {
                using (OpidDB opidcontext = new OpidDB())
                {
                    foreach (Check check in checks)
                    {
                        RCheck existing = opidcontext.RChecks.Where(u => u.Num == check.Num).FirstOrDefault();

                        if (existing == null) // && string.IsNullOrEmpty(check.Clr))
                        {
                            RCheck unresolved = new RCheck
                            {
                                RecordID = check.RecordID,
                                sRecordID = check.RecordID.ToString(),
                                InterviewRecordID = check.InterviewRecordID,
                                sInterviewRecordID = check.InterviewRecordID.ToString(),
                                Num = check.Num,
                                sNum = check.Num.ToString(),
                                Name = check.Name,
                                Date = check.Date,
                                Service = check.Service,
                                Disposition = check.Disposition,
                                Matched = false
                            };

                            opidcontext.RChecks.Add(unresolved);
                        }
                        else if (!string.IsNullOrEmpty(check.Disposition))
                        {
                            // The existing check may have its disposition
                            // changed to, for example, Voided/Replaced.
                            // If a file of voided checks contains a check with number existing.Num
                            // then this change of disposition will protect this check from having its status
                            // in Apricot changed from Voided/Replaced to Voided
                            existing.Disposition = check.Disposition;
                        }
                    }

                    opidcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                int z;
                z = 2;
            }
        }


        public static List<CheckViewModel> GetChecks()
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var pchecks = (from check in opidcontext.RChecks select check).ToList();

                List<CheckViewModel> checks = new List<CheckViewModel>();

                foreach (RCheck rc in pchecks)
                {
                    checks.Add(new CheckViewModel
                    {
                        RecordID = rc.RecordID,
                        InterviewRecordID = rc.InterviewRecordID,
                        Num = rc.Num,
                        Name = rc.Name,
                        Date = rc.Date, //rc.Date.ToShortDateString(),
                        Service = rc.Service,
                        Disposition = rc.Disposition
                    });
                }

                return checks;
            }
        }

        public static bool ResearchTableIsEmpty()
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var checks = opidcontext.RChecks;

                if (checks.Count() == 0) // Is the table empty for a restore operation?
                {
                    return true;
                }
            }

            return false;
        }
 
        public static string GetResearchTableName()
        {
            string timestamp = Extras.GetTimestamp();
            string fname = string.Format("Research {0}", timestamp);
            return fname;
        }  

        public static string GetResearchTableCSV()
        {
            List<CheckViewModel> checks = DataManager.GetChecks();
            var csv = new StringBuilder();

            // N.B. No spaces between column names in the header row!
            string header = "Date,Record ID,Interview Record ID,Name,Check Number,Service,Disposition";
            csv.AppendLine(header);

            foreach (CheckViewModel check in checks)
            {
                string csvrow = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    check.Date,
                    check.RecordID,
                    check.InterviewRecordID,
                    string.Format("\"{0}\"", check.Name),
                    check.Num,
                    check.Service,
                    check.Disposition);

                csv.AppendLine(csvrow);
            }

            return csv.ToString();
        }

        public static void RestoreResearchTable(string rtFileName)
        {
            string pathToResearchTableFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", rtFileName));

            List<CheckViewModel> rchecks = MyExcelDataReader.GetCVMS(pathToResearchTableFile);
          
            RestoreRChecksTable(rchecks);
        }

        private static void RestoreRChecksTable(List<CheckViewModel> rChecks)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var checks = opidcontext.RChecks;
               
                if (checks.Count() == 0) // Is the table empty for rebuild?
                {
                    foreach (CheckViewModel rc in rChecks)
                    {
                        
                        checks.Add(new RCheck
                        {
                            RecordID = rc.RecordID,
                            sRecordID = rc.sRecordID,
                            InterviewRecordID = rc.InterviewRecordID,
                            sInterviewRecordID = rc.sInterviewRecordID,
                            Num = rc.Num,
                            sNum = rc.sNum,
                            Name = rc.Name,
                            Date = Convert.ToDateTime(rc.Date), 
                            sDate = Convert.ToDateTime(rc.Date).ToString("MM/dd/yyyy"),
                            Service = rc.Service,
                            Disposition = rc.Disposition
                        });
                    }

                    opidcontext.SaveChanges();
                    return;
                }
            }
        }

        public static List<DispositionRow> GetResearchRows(string uploadedFile)
        {
            // List<DispositionRow> originalRows = new List<DispositionRow>();
            //  string pathToApricotReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Public/{0}.{1}", apFileName, apFileType));
            string pathToResearchReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", uploadedFile));

            List<DispositionRow> resRows = MyExcelDataReader.GetResearchRows(pathToResearchReportFile);

            return resRows;
        }
    }
}