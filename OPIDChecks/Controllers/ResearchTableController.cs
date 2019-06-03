using OPIDChecks.DAL;
using OPIDChecks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Web.Helpers;
using OPIDEntities;

namespace OPIDChecks.Controllers
{
    public class ResearchTableController : Controller
    { 
       
        
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

        public ActionResult ResearchTable()
        {
            return View();
        }

        /*
        public ActionResult Backup()
        {
            FileDownloader.DownloadResearchTable();
            return View("ResearchTable");
        }
        */

        [HttpPost]
        public JsonResult Backup()
        {
            string researchTableFileName = FileDownloader.DownloadResearchTable();
            return Json(new
            {
                statusCode = 200,
                status = "File uploaded",
                rtfile = researchTableFileName,
            }, "text/html");
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

                List<string> docfiles = DataManager.UploadFile(postedFile);
                 
                DataManager.RestoreResearchTable(postedFile.FileName);
                return View("ResearchTable", model);

            }

            return View(model);
        }
    }
}