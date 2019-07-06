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

        private static List<Check> newResearchChecks;
        private static List<CheckViewModel> resolvedChecks;
        private static List<int> mistakenlyResolved;
        private static List<Check> typoChecks;

        public static void Init()
        {
            if (firstCall)
            {
                typoChecks = new List<Check>();
                resolvedChecks = new List<CheckViewModel>();
                mistakenlyResolved = new List<int>();
                firstCall = false;
            }

            newResearchChecks = new List<Check>();
            incidentals = new List<int>();
        }

        public static IQueryable<CheckViewModel> GetResolvedChecksAsQueryable()
        {
            if (resolvedChecks == null)
            {
                List<CheckViewModel> emptyList = new List<CheckViewModel>();
                return emptyList.AsQueryable();
            }

            return resolvedChecks.AsQueryable();
        }

        public static List<CheckViewModel> GetResolvedChecksAsList()
        {
            return resolvedChecks;
        }

        public static void PersistResearchChecks(List<DispositionRow> researchRows)
        {
            List<Check> rChecks = DetermineResearchChecks(researchRows);
            AppendToResearchChecks(rChecks);
        }

        public static void NewResearchCheck(DispositionRow row, string service, string disposition)
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

            newResearchChecks.Add(new Check
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

        private static List<Check> DetermineResearchChecks(List<DispositionRow> researchRows)
        {
            foreach (DispositionRow row in researchRows)
            {
                if (row.LBVDCheckNum != 0)
                {
                    NewResearchCheck(row, "LBVD", row.LBVDCheckDisposition);
                }

                if (row.LBVDCheckNum2 != 0)
                {
                    NewResearchCheck(row, "LBVD2", row.LBVDCheck2Disposition);
                }

                if (row.LBVDCheckNum3 != 0)
                {
                    NewResearchCheck(row, "LBVD3", row.LBVDCheck3Disposition);
                }

                if (row.TIDCheckNum != 0)
                {
                    NewResearchCheck(row, "TID", row.TIDCheckDisposition);
                }

                if (row.TIDCheckNum2 != 0)
                {
                    NewResearchCheck(row, "TID2", row.TIDCheck2Disposition);
                }

                if (row.TIDCheckNum3 != 0)
                {
                    NewResearchCheck(row, "TID3", row.TIDCheck3Disposition);
                }

                if (row.TDLCheckNum != 0)
                {
                    NewResearchCheck(row, "TDL", row.TDLCheckDisposition);
                }

                if (row.TDLCheckNum2 != 0)
                {
                    NewResearchCheck(row, "TDL2", row.TDLCheck2Disposition);
                }

                if (row.TDLCheckNum3 != 0)
                {
                    NewResearchCheck(row, "TDL3", row.TDLCheck3Disposition);
                }

                if (row.MBVDCheckNum != 0)
                {
                    NewResearchCheck(row, "MBVD", row.MBVDCheckDisposition);
                }

                if (row.MBVDCheckNum2 != 0)
                {
                    NewResearchCheck(row, "MBVD2", row.MBVDCheck2Disposition);
                }

                if (row.MBVDCheckNum3 != 0)
                {
                    NewResearchCheck(row, "MBVD3", row.MBVDCheck3Disposition);
                }

                if (row.SDCheckNum != 0)
                {
                    NewResearchCheck(row, "SD", row.SDCheckDisposition);
                }
            }

            return newResearchChecks;
        }
        
        private static void AppendToResearchChecks(List<Check> checks)
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
                            RCheck rcheck = new RCheck
                            {
                                RecordID = check.RecordID,
                                sRecordID = check.RecordID.ToString(),
                                InterviewRecordID = check.InterviewRecordID,
                                sInterviewRecordID = check.InterviewRecordID.ToString(),
                                Num = check.Num,
                                sNum = check.Num.ToString(),
                                Name = check.Name,
                                Date = check.Date,
                                sDate = check.Date.ToString("MM/dd/yyyy"),
                                Service = check.Service,
                                Disposition = check.Disposition,
                            };

                            opidcontext.RChecks.Add(rcheck);
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

        public static List<Check> GetResearchChecks()
        {
            List<Check> researchChecks = new List<Check>();

            using (OpidDB opidcontext = new OpidDB())
            {
                List<RCheck> rchecks = opidcontext.RChecks.Select(u => u).ToList();

                foreach (var lu in rchecks)
                {
                    researchChecks.Add(new Check
                    {
                        RecordID = lu.RecordID,
                        InterviewRecordID = lu.InterviewRecordID,
                        Num = lu.Num,
                        Name = lu.Name,
                        Date = lu.Date,
                        Service = lu.Service,
                        Disposition = lu.Disposition,
                    });
                }
            }

            return researchChecks;
        }

        public static void ResolveResearchChecks()
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var researchChecks = opidcontext.RChecks;

                foreach (CheckViewModel check in resolvedChecks)
                {
                    List<RCheck> rchecks = researchChecks.Where(u => u.Num == check.Num || u.Num == -check.Num).ToList();

                    foreach (RCheck rcheck in rchecks)
                    {
                        rcheck.Disposition = check.Disposition;
                    }
                }

                opidcontext.SaveChanges();
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

        public static void DeleteResearchTable()
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                opidcontext.RChecks.RemoveRange(opidcontext.RChecks);  // Remove all checks from table RChecks (the Research Table)
                opidcontext.SaveChanges();
                return;
            }
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
            string pathToResearchReportFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", uploadedFile));
            List<DispositionRow> resRows = MyExcelDataReader.GetResearchRows(pathToResearchReportFile);
            return resRows;
        }
 

        public static List<Check> GetExcelChecks(string uploadedFileName, string disposition)
        {
            if (uploadedFileName.Equals("unknown"))
            {
                // Return an emmpty list of checks.
                return new List<Check>();
            }

            string pathToUploadedChecksFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", uploadedFileName));

            List<Check> excelChecks = MyExcelDataReader.GetExcelChecks(pathToUploadedChecksFile);

            foreach (Check check in excelChecks)
            {
                // Implicit status of voided checks is "Voided"
                // check.Clr = "Voided";
                check.Disposition = disposition;
            }

            return excelChecks;
        }

        public static void NewResolvedCheck(Check check, string disposition)
        {
            // PLB 1/23/2019 Added r.RecordID == check.RecordID.
            // This fxed the problem that Bill reported in an email dated 1/21/2019.
            CheckViewModel alreadyResolved = resolvedChecks.Where(r => (r.RecordID == check.RecordID && (r.Num == check.Num || r.Num == -check.Num))).FirstOrDefault();
            CheckViewModel cvm = null;

            if (alreadyResolved == null)
            {
                cvm = new CheckViewModel
                {
                    RecordID = check.RecordID,
                    sRecordID = check.RecordID.ToString(),
                    InterviewRecordID = check.InterviewRecordID,
                    sInterviewRecordID = check.InterviewRecordID.ToString(),
                    Name = check.Name,
                    Num = check.Num,
                    sNum = check.Num.ToString(),
                    Date = check.Date,
                    sDate = check.Date.ToString("MM/dd/yyyy"),
                    Service = check.Service,
                    Disposition = disposition
                };

                resolvedChecks.Add(cvm);
            }
        }
    }
}