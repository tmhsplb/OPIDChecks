﻿using OPIDChecks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDChecks.DAL
{
    public class Merger
    {
        public static void PerformMerge(string uploadedFile, string fileType)
        {
            switch (fileType)
            {
                case "InterviewResearch":
                    UpdateResearchTableFromInterviewResearchFile(uploadedFile);
                    break;

                case "VoidedChecks":
                    //UpdateResearchTableFromVoidedChecksFile(uploadedFile);
                    UpdateResearchTableFromExcelChecksFile(uploadedFile, "Voided");

                    break;

                case "ClearedChecks":
                   // UpdateResearchTableFromClearedChecksFile(uploadedFile);
                    UpdateResearchTableFromExcelChecksFile(uploadedFile, "Cleared");
                    break;

                default:
                    break;
            }
        }

        public static bool IsProtectedCheck(string disposition)
        {
            if (string.IsNullOrEmpty(disposition))
            {
                return false;
            }

            return disposition.Equals("Voided/Replaced")
                || disposition.Equals("Voided/Reissued")
                || disposition.Equals("Voided/No Reissue")
                || disposition.Equals("Voided/Reissue Other")
                || disposition.Equals("Scammed Check");
        }

        private static void DetermineResolvedChecks(List<Check> checks, string disposition, List<Check> researchChecks)
        {
            foreach (Check check in checks)
            {
                List<Check> matchedChecks = researchChecks.FindAll(c => c.Num == check.Num || c.Num == -check.Num);

                // Normally, matchedChecks.Count() == 0 or matchedChecks.Count == 1 
                // But in the case of a birth certificate, a single check number may cover
                // multiple children. In this case matchedChecks.Count() > 1.
                // The foreach loop below creates a new resolved check for each matched check.
                // This means that if one check number is used by a parent and his/her children,
                // then there will be a resolved check for the parent and each child.
                if (matchedChecks.Count() != 0)
                {
                    foreach (Check matchedCheck in matchedChecks)
                    {
                        bool protectedCheck = IsProtectedCheck(matchedCheck.Disposition);

                       // string disposition = (type.Equals("ImportMe") ? check.Disposition : type);

                        if (!protectedCheck)
                        {
                            DataManager.NewResolvedCheck(matchedCheck, disposition);
                        }

                        /* Operation Recovery code
                        if (type.Equals("ImportMe"))
                        {
                            // DataManager.RecoverLostChecks(check, researchChecks);
                        }
                        */
                    }
                }
                /*
                else // PLB 1/11/2019
                {
                    // Operation Recovery indavertently erased some level 2 checks (LBVD2, TID2, etc.)
                    // This code restores the lost check numbers and adds the erased checks as nameless
                    // checks in the Research Table. The lost checks are entered through the Operation Recovery - Dec 2018
                    // entry on the Merge screen.
                    DataManager.AppendResearchCheck(check);
                    DataManager.NewResolvedCheck(check, check.Disposition);
                }
                */
            }
        }

        public static void UpdateResearchTableFromInterviewResearchFile(string uploadedFile)
        {
            List<DispositionRow> researchRows = DataManager.GetResearchRows(uploadedFile);

            DataManager.Init();

            // Handle incidental checks before persisting unmatched checks.
            // This way an Interview Research file cannot add to the set
            // of resolved checks by mistake.
            // For example, the Interview Research File may contain both
            //    Estes, Jason  TID = 74726, TID Disposition = Voided/Replaced
            //    Justice, Mark TID = 74726, TID Disposition = ?
            // In this case, check number 74726 was mistakenly assigned to both
            // the TID for Jason Estes and the TID for Mark Justice.
            // If incidental checks are handled after unmatched checks are persisted,
            // then the check for Jason Estes will resolve the check for Mark Justice.
            // We don't want this to happen! Most likely, the check number 74726
            // for Mark Justice was a typo.
            // PLB 12/14/2018 DataManager.HandleIncidentalChecks(researchRows);
            DataManager.PersistResearchChecks(researchRows);
            //  PLB 12/14/2018 Don't call RemoveTypoChecks
            // DataManager.RemoveTypoChecks();
        }

        /*
        public static void UpdateResearchTableFromVoidedChecksFile(string uploadedFile)
        {
            DataManager.Init();
            List<Check> voidedChecks = DataManager.GetVoidedChecks(uploadedFile);
            List<Check> researchChecks = DataManager.GetResearchChecks();

            DetermineResolvedChecks(voidedChecks, "Voided", researchChecks);
            DataManager.ResolveResearchChecks();
        }

        public static void UpdateResearchTableFromClearedChecksFile(string uploadedFile)
        {
            DataManager.Init();
            List<Check> clearedChecks = DataManager.GetClearedChecks(uploadedFile);
            List<Check> researchChecks = DataManager.GetResearchChecks();

            DetermineResolvedChecks(clearedChecks, "Cleared", researchChecks);
            DataManager.ResolveResearchChecks();
        }
        */

        public static void UpdateResearchTableFromExcelChecksFile(string uploadedFile, string disposition)
        {
            DataManager.Init();
            
            List<Check> excelChecks = DataManager.GetExcelChecks(uploadedFile, disposition);
            List<Check> researchChecks = DataManager.GetResearchChecks();

            DetermineResolvedChecks(excelChecks, disposition, researchChecks);
            DataManager.ResolveResearchChecks();
        }
    }
}