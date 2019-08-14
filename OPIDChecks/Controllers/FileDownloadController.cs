﻿using OPIDChecks.DAL;
using OPIDChecks.Models;
using OPIDChecks.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OPIDChecks.Controllers
{
    public class FileDownloadController : Controller
    {
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

        [HttpPost]
        public JsonResult GetResearchTable()
        {

            string researchTableFileName = Extras.GetResearchTableName();
            string content = GetResearchTableCSV();

            return Json(new
            {
                rtFileName = researchTableFileName,
                content = content
            }, "text/html");
        }

        private static void PrepopulateImportRow(List<Check> researchChecks, ImportRow importRow)
        {
            List<Check> rChecks = researchChecks.FindAll(r => r.RecordID == importRow.RecordID && r.InterviewRecordID == importRow.InterviewRecordID).ToList();

            foreach (Check resolvedCheck in rChecks)
            {
                CheckViewModel cvm = new CheckViewModel();
                cvm.Num = resolvedCheck.Num;
                cvm.Disposition = resolvedCheck.Disposition;
                cvm.Service = resolvedCheck.Service;
                 
                PopulateImportRow(cvm, importRow);
            }
        }

        private static void PopulateImportRow(CheckViewModel resolvedCheck, ImportRow importRow)
        {
            switch (resolvedCheck.Service)
            {
                case "LBVD":
                    importRow.LBVDCheckNum = resolvedCheck.Num;
                    importRow.LBVDCheckDisposition = resolvedCheck.Disposition;
                    break;
                case "LBVD2":
                    importRow.LBVDCheckNum2 = resolvedCheck.Num;
                    importRow.LBVDCheck2Disposition = resolvedCheck.Disposition;
                    break;
                case "LBVD3":
                    importRow.LBVDCheckNum2 = resolvedCheck.Num;
                    importRow.LBVDCheck3Disposition = resolvedCheck.Disposition;
                    break;

                case "TID":
                    importRow.TIDCheckNum = resolvedCheck.Num;
                    importRow.TIDCheckDisposition = resolvedCheck.Disposition;
                    break;
                case "TID2":
                    importRow.TIDCheckNum2 = resolvedCheck.Num;
                    importRow.TIDCheck2Disposition = resolvedCheck.Disposition;
                    break;
                case "TID3":
                    importRow.TIDCheckNum3 = resolvedCheck.Num;
                    importRow.TIDCheck3Disposition = resolvedCheck.Disposition;
                    break;

                case "TDL":
                    importRow.TDLCheckNum = resolvedCheck.Num;
                    importRow.TDLCheckDisposition = resolvedCheck.Disposition;
                    break;
                case "TDL2":
                    importRow.TDLCheckNum2 = resolvedCheck.Num;
                    importRow.TDLCheck2Disposition = resolvedCheck.Disposition;
                    break;
                case "TDL3":
                    importRow.TDLCheckNum3 = resolvedCheck.Num;
                    importRow.TDLCheck3Disposition = resolvedCheck.Disposition;
                    break;

                case "MBVD":
                    importRow.MBVDCheckNum = resolvedCheck.Num;
                    importRow.MBVDCheckDisposition = resolvedCheck.Disposition;
                    break;
                case "MBVD2":
                    importRow.MBVDCheckNum2 = resolvedCheck.Num;
                    importRow.MBVDCheck2Disposition = resolvedCheck.Disposition;
                    break;
                case "MBVD3":
                    importRow.MBVDCheckNum2 = resolvedCheck.Num;
                    importRow.MBVDCheck3Disposition = resolvedCheck.Disposition;
                    break;

                case "SD":
                    importRow.SDCheckNum = resolvedCheck.Num;
                    importRow.SDCheckDisposition = resolvedCheck.Disposition;
                    break;
            }
        }

        private static ImportRow NewImportRow(List<Check> researchChecks, CheckViewModel resolvedCheck, string disposition)
        {
            ImportRow importRow = new ImportRow
            {
                RecordID = resolvedCheck.RecordID,
                InterviewRecordID = resolvedCheck.InterviewRecordID
            };

            PrepopulateImportRow(researchChecks, importRow);

            PopulateImportRow(resolvedCheck, importRow);

            return importRow;
        }

        private static void UpdateExistingImportRow(CheckViewModel resolvedCheck, string disposition, ImportRow irow)
        {
            int checkNum = resolvedCheck.Num;

            switch (resolvedCheck.Service)
            {
                case "LBVD":
                    if (irow.LBVDCheckNum == 0)
                    {
                        irow.LBVDCheckNum = checkNum;
                        irow.LBVDCheckDisposition = disposition;
                    }
                    break;
                case "LBVD2":
                    if (irow.LBVDCheckNum2 == 0)
                    {
                        irow.LBVDCheckNum2 = checkNum;
                        irow.LBVDCheck2Disposition = disposition;
                    }
                    break;
                case "LBVD3":
                    if (irow.LBVDCheckNum3 == 0)
                    {
                        irow.LBVDCheckNum3 = checkNum;
                        irow.LBVDCheck3Disposition = disposition;
                    }
                    break;
                case "TID":
                    if (irow.TIDCheckNum == 0)
                    {
                        irow.TIDCheckNum = checkNum;
                        irow.TIDCheckDisposition = disposition;
                    }
                    break;
                case "TID2":
                    if (irow.TIDCheckNum2 == 0)
                    {
                        irow.TIDCheckNum2 = checkNum;
                        irow.TIDCheck2Disposition = disposition;
                    }
                    break;
                case "TID3":
                    if (irow.TIDCheckNum3 == 0)
                    {
                        irow.TIDCheckNum3 = checkNum;
                        irow.TIDCheck3Disposition = disposition;
                    }
                    break;
                case "TDL":
                    if (irow.TDLCheckNum == 0)
                    {
                        irow.TDLCheckNum = checkNum;
                        irow.TDLCheckDisposition = disposition;
                    }
                    break;
                case "TDL2":
                    if (irow.TDLCheckNum2 == 0)
                    {
                        irow.TDLCheckNum2 = checkNum;
                        irow.TDLCheck2Disposition = disposition;
                    }
                    break;
                case "TDL3":
                    if (irow.TDLCheckNum3 == 0)
                    {
                        irow.TDLCheckNum3 = checkNum;
                        irow.TDLCheck3Disposition = disposition;
                    }
                    break;
                case "MBVD":
                    if (irow.MBVDCheckNum == 0)
                    {
                        irow.MBVDCheckNum = checkNum;
                        irow.MBVDCheckDisposition = disposition;
                    }
                    break;
                case "MBVD2":
                    if (irow.MBVDCheckNum2 == 0)
                    {
                        irow.MBVDCheckNum2 = checkNum;
                        irow.MBVDCheck2Disposition = disposition;
                    }
                    break;
                case "MBVD3":
                    if (irow.MBVDCheckNum3 == 0)
                    {
                        irow.MBVDCheckNum3 = checkNum;
                        irow.MBVDCheck3Disposition = disposition;
                    }
                    break;
                case "SD":
                    if (irow.SDCheckNum == 0)
                    {
                        irow.SDCheckNum = checkNum;
                        irow.SDCheckDisposition = disposition;
                    }
                    break;
                default:
                    break;
            }
        }

        public static List<ImportRow> GetImportRows()
        {
            List<Check> researchChecks = DataManager.GetResearchChecks();
            List<ImportRow> importRows = new List<ImportRow>();

            // Each resolved check creates a new import row or updates an existing one.
            foreach (CheckViewModel resolvedCheck in DataManager.GetResolvedChecksAsList())
            {
                string disposition = resolvedCheck.Disposition;   //GetDispositionFromCheck(resolvedCheck);

                if (disposition != null && !disposition.Equals("Unknown"))
                {
                    List<ImportRow> irows = (from irow in importRows
                                             where irow.LBVDCheckNum == resolvedCheck.Num
                                                   || irow.LBVDCheckNum2 == resolvedCheck.Num
                                                   || irow.LBVDCheckNum3 == resolvedCheck.Num
                                                   || irow.TIDCheckNum == resolvedCheck.Num
                                                   || irow.TIDCheckNum2 == resolvedCheck.Num
                                                   || irow.TIDCheckNum3 == resolvedCheck.Num
                                                   || irow.TDLCheckNum == resolvedCheck.Num
                                                   || irow.TDLCheckNum2 == resolvedCheck.Num
                                                   || irow.TDLCheckNum3 == resolvedCheck.Num
                                                   || irow.MBVDCheckNum == resolvedCheck.Num
                                                   || irow.MBVDCheckNum2 == resolvedCheck.Num
                                                   || irow.MBVDCheckNum3 == resolvedCheck.Num
                                                   || irow.SDCheckNum == resolvedCheck.Num

                                                   // Does resolvedCheck match an existing importRow by ID?
                                                   // This is the case where there is more than one check on an import row, IR, 
                                                   // and resolvedCheck will be used to update row IR.
                                                   || (resolvedCheck.InterviewRecordID != 0 && irow.InterviewRecordID == resolvedCheck.InterviewRecordID)
                                                   || (resolvedCheck.RecordID != 0 && irow.RecordID == resolvedCheck.RecordID)
                                             select irow).ToList();

                    if (irows.Count() == 0)
                    {
                        // There is no import row representing this resolved check.
                        // Create one.
                        importRows.Add(NewImportRow(researchChecks, resolvedCheck, disposition));
                    }
                    else
                    {
                        bool added = false;

                        foreach (ImportRow irow in irows)
                        {
                            if ((resolvedCheck.Service == "LBVD" || resolvedCheck.Service == "LBVD2" || resolvedCheck.Service == "LBVD3"
                                || resolvedCheck.Service == "TID" || resolvedCheck.Service == "TID2" || resolvedCheck.Service == "TID3"
                                || resolvedCheck.Service == "TDL" || resolvedCheck.Service == "TDL2" || resolvedCheck.Service == "TDL3"
                                || resolvedCheck.Service == "MBVD" || resolvedCheck.Service == "MBVD2" || resolvedCheck.Service == "MBVD3")
                                &&
                                ((resolvedCheck.InterviewRecordID != 0 && resolvedCheck.InterviewRecordID != irow.InterviewRecordID)
                                ||
                                (resolvedCheck.RecordID != 0 && resolvedCheck.RecordID != irow.RecordID)))
                            {
                                // Case of same check number being used for multiple
                                // birth certificates.
                                if (!added)
                                {
                                    importRows.Add(NewImportRow(researchChecks, resolvedCheck, disposition));
                                    // Prevent the same resolved check from being added twice.
                                    added = true;
                                }
                            }
                            else
                            {
                                // Found row among existing import rows. There is more than one check
                                // number on this row. In other words, the client had more than
                                // one check written for the visit this row corresponds to.
                                UpdateExistingImportRow(resolvedCheck, disposition, irow);
                            }
                        }
                    }
                } 
            }

            return importRows;
        }


        public static string GetImportMeFileCSV()
        {
            var csv = new StringBuilder();

            string pathToDispositionHeader = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/App_Data/Interview Import Me Header.csv"));

            using (StreamReader reader = new StreamReader(pathToDispositionHeader))
            {
                string header = reader.ReadToEnd();
                csv.Append(header);
            }

            List<ImportRow> importRows = GetImportRows();

            foreach (ImportRow d in importRows)
            {
                if (d.LBVDCheckNum > 0 || d.LBVDCheckNum2 > 0 || d.LBVDCheckNum3 > 0
                    || d.TIDCheckNum > 0 || d.TIDCheckNum2 > 0 || d.TIDCheckNum3 > 0
                    || d.TDLCheckNum > 0 || d.TDLCheckNum2 > 0 || d.TDLCheckNum3 > 0
                    || d.MBVDCheckNum > 0 || d.MBVDCheckNum2 > 0 || d.MBVDCheckNum3 > 0
                    || d.SDCheckNum > 0)
                {
                    // Only create a row if it contains a modified check number.
                    string csvRow = string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24}", // ,{9},{10}",
                        d.InterviewRecordID,
                        (d.LBVDCheckNum > 0 ? d.LBVDCheckNum : 0),
                        (d.LBVDCheckNum > 0 ? d.LBVDCheckDisposition : string.Empty),
                        (d.LBVDCheckNum2 > 0 ? d.LBVDCheckNum2 : 0),
                        (d.LBVDCheckNum2 > 0 ? d.LBVDCheck2Disposition : string.Empty),
                        (d.LBVDCheckNum3 > 0 ? d.LBVDCheckNum3 : 0),
                        (d.LBVDCheckNum3 > 0 ? d.LBVDCheck3Disposition : string.Empty),

                        (d.TIDCheckNum > 0 ? d.TIDCheckNum : 0),
                        (d.TIDCheckNum > 0 ? d.TIDCheckDisposition : string.Empty),
                        (d.TIDCheckNum2 > 0 ? d.TIDCheckNum2 : 0),
                        (d.TIDCheckNum2 > 0 ? d.TIDCheck2Disposition : string.Empty),
                        (d.TIDCheckNum3 > 0 ? d.TIDCheckNum3 : 0),
                        (d.TIDCheckNum3 > 0 ? d.TIDCheck3Disposition : string.Empty),

                        (d.TDLCheckNum > 0 ? d.TDLCheckNum : 0),
                        (d.TDLCheckNum > 0 ? d.TDLCheckDisposition : string.Empty),
                        (d.TDLCheckNum2 > 0 ? d.TDLCheckNum2 : 0),
                        (d.TDLCheckNum2 > 0 ? d.TDLCheck2Disposition : string.Empty),
                        (d.TDLCheckNum3 > 0 ? d.TDLCheckNum3 : 0),
                        (d.TDLCheckNum3 > 0 ? d.TDLCheck3Disposition : string.Empty),

                        (d.MBVDCheckNum > 0 ? d.MBVDCheckNum : 0),
                        (d.MBVDCheckNum > 0 ? d.MBVDCheckDisposition : string.Empty),
                        (d.MBVDCheckNum2 > 0 ? d.MBVDCheckNum2 : 0),
                        (d.MBVDCheckNum2 > 0 ? d.MBVDCheck2Disposition : string.Empty),
                        (d.MBVDCheckNum3 > 0 ? d.MBVDCheckNum3 : 0),
                        (d.MBVDCheckNum3 > 0 ? d.MBVDCheck3Disposition : string.Empty));
                    //     (d.SDCheckNum > 0 ? d.SDCheckNum : 0),
                    //     (d.SDCheckNum > 0 ? d.SDCheckDisposition : string.Empty));

                    csv.AppendLine(csvRow);
                }
            }

            return csv.ToString();
        }

        public JsonResult GetImportMeFile()
        {
            string importMeFileName = Extras.GetImportMeFileName();
            string content = GetImportMeFileCSV();

            return Json(new
            {
                importMeFileName = importMeFileName,
                content = content
            }, "text/html");
        }
    }
}