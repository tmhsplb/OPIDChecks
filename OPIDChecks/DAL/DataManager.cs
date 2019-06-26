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
           // List<Check> unmatchedChecks = DetermineUnmatchedChecks(researchRows);
            AppendToUnresolvedChecks(unmatchedChecks);
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
                                InterviewRecordID = check.InterviewRecordID,
                                Num = check.Num,
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
                        Date = rc.Date.ToShortDateString(),
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