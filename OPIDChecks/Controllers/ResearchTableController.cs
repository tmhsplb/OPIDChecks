using OPIDChecks.DAL;
using OPIDChecks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Web.Helpers;
using OPIDEntities;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using OPIDChecks.DataContexts;
using DataTables.Mvc;
using OPIDChecks.Utils;
using System.Threading;

namespace OPIDChecks.Controllers
{
    public class ResearchTableController : Controller
    {
        public ActionResult ResearchTable()
        {
            return View();
        }

        // Don't know how to move this to the DataManager where it belongs because of return type issues.
        // Just leave it here for now.
        public JsonResult GetChecks([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                IQueryable<RCheck> query = opidcontext.RChecks;

                /* Used when searching for bottleneck.
                IQueryable<RCheck> query = opidcontext.RChecks.Where(c => c.Name.StartsWith("A") || c.Name.StartsWith("B") || c.Name.StartsWith("C") || c.Name.StartsWith("D")
                  || c.Name.StartsWith("E") || c.Name.StartsWith("F") || c.Name.StartsWith("G") || c.Name.StartsWith("H") || c.Name.StartsWith("I") || c.Name.StartsWith("J")
                  || c.Name.StartsWith("K") || c.Name.StartsWith("L") || c.Name.StartsWith("M") || c.Name.StartsWith("N") || c.Name.StartsWith("O") || c.Name.StartsWith("P")
                  || c.Name.StartsWith("Q") || c.Name.StartsWith("R") || c.Name.StartsWith("S") || c.Name.StartsWith("T") || c.Name.StartsWith("U") || c.Name.StartsWith("V")
                  || c.Name.StartsWith("W") || c.Name.StartsWith("X") || c.Name.StartsWith("Y") || c.Name.StartsWith("Z"));
                */

                var totalCount = query.Count();

                // Apply filters for searching
                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();
                    query = query.Where(p => p.Name.Contains(value) ||
                                             p.sRecordID.Contains(value) ||
                                             p.sInterviewRecordID.Contains(value) ||
                                             p.sNum.Contains(value) ||
                                             p.sDate.Contains(value) ||
                                             p.Service.Contains(value) ||
                                             p.Disposition.Contains(value)
                                       );
                }

                var filteredCount = query.Count();

                var fqo = query.OrderBy("Id asc");  // Order by the primary key for speed. Oredering by Name times out, because Name is not an indexed field.

                // Paging
                var fqop = fqo.Skip(requestModel.Start).Take(requestModel.Length);
                
                var data = fqop.Select(rcheck => new
                {
                    sRecordID = rcheck.sRecordID,
                    sInterviewRecordID = rcheck.sInterviewRecordID,
                    Name = rcheck.Name,
                    sNum = rcheck.sNum,
                  //  Date = rcheck.Date,
                    sDate = rcheck.sDate,
                    Service = rcheck.Service,
                    Disposition = rcheck.Disposition
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteResearchTable()
        {
            DataManager.DeleteResearchTable();
            return View("ResearchTable");
        }

        /*
        // Used for testing modal progress bar measuring the progress of restoring a research table.
        // Based on code found at:
        //  https://www.codeproject.com/articles/1124691/signalr-progress-bar-simple-example-sending-live-d
        [HttpPost]
        public ActionResult Restore(FileViewModel model)
        {
            //THIS COULD BE SOME LIST OF DATA
            int itemsCount = 100;

            for (int i = 0; i <= itemsCount; i++)
            {
                //SIMULATING SOME TASK
                Thread.Sleep(500);

                //CALLING A FUNCTION THAT CALCULATES PERCENTAGE AND SENDS THE DATA TO THE CLIENT
                ProgressHub.SendProgress("Process in progress...", i, itemsCount);
            }

            return View("ResearchTable", model);
        }
        */

        
        [HttpPost]
        public ActionResult Restore(FileViewModel model)
        {
            if (!DataManager.ResearchTableIsEmpty())
            {
                ModelState.AddModelError("", "The Research Table must be empty before a restore is performed.");
                return View("ResearchTable", model);
            }

            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                if (!postedFile.FileName.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("", "This is not an Excel xlsx file.");
                    return View("ResearchTable", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                 
                DataManager.RestoreResearchTable(postedFile.FileName);
               
                return View("ResearchTable", model);
            }

            return View(model);
        }
    }
}