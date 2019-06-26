using OPIDChecks.Models;
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

                default:
                    break;
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
            DataManager.PersistUnmatchedChecks(researchRows);
            //  PLB 12/14/2018 Don't call RemoveTypoChecks
            // DataManager.RemoveTypoChecks();
        }
    }
}