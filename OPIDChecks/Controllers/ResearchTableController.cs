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

namespace OPIDChecks.Controllers
{
    public class ResearchTableController : Controller
    { 
       
        [HttpPost]
        public JsonResult GetResearchTable()
        {
            
            string researchTableFileName = DataManager.GetResearchTableName();
            string content = DataManager.GetResearchTableCSV();

            return Json(new
            {
                rtFileName = researchTableFileName,
                content = content
            }, "text/html");
        }

        public ActionResult ResearchTable()
        {
            return View();
           // return View("MVC5ResearchTable");
        }

        // Don't know how to move this to the DataManager where it belongs because of return type issues.
        // Just leave it here for now.
        public JsonResult GetChecks([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                IQueryable<RCheck> query = opidcontext.RChecks;
                var totalCount = query.Count();

                 
                // Apply filters for searching
                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();
                    query = query.Where(p => p.Name.Contains(value) ||
                                             p.RecordID.Contains(value) ||
                                             p.InterviewRecordID.Contains(value) ||
                                             p.Num.Contains(value) ||
                                             p.Service.Contains(value) ||
                                             p.Disposition.Contains(value)
                                       );
                }

                var filteredCount = query.Count();

                query = query.OrderBy("Name asc");

                // Paging
                query = query.Skip(requestModel.Start).Take(requestModel.Length);
                
                var data = query.Select(rcheck => new
                {
                    RecordID = rcheck.RecordID,
                    InterviewRecordID = rcheck.InterviewRecordID,
                    Name = rcheck.Name,
                    Num = rcheck.Num,
                    Date = rcheck.Date,
                    Service = rcheck.Service,
                    Disposition = rcheck.Disposition
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
            }
        }

        /*
        public JsonResult GetChecks()
        {
            List<CheckViewModel> checks = DataManager.GetChecks();

            // See https://www.codeproject.com/Tips/1011531/Using-jQuery-DataTables-with-Server-Side-Processing
            var jsonData = new
            {
                draw = 1,
                recordsTotal = checks.Count,
                recordsFiltered = checks.Count,
                data = checks
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
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

                List<string> docfiles = DataManager.UploadFile(postedFile);
                 
                DataManager.RestoreResearchTable(postedFile.FileName);
                return View("ResearchTable", model);
            }

            return View(model);
        }
    }
}