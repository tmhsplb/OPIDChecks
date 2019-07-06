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

                query = query.OrderBy("Name asc");

                // Paging
                query = query.Skip(requestModel.Start).Take(requestModel.Length);
                
                var data = query.Select(rcheck => new
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