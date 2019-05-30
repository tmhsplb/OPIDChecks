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
       
        public JsonResult GetChecks(int draw, int start, int length)
        {
            int sortColumn = -1;
            string sortColumnDir = "asc";

            //Find Order Column
           // var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
           // var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            if (Request.QueryString["order[0][column]"] != null)
            {
                sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            }
            if (Request.QueryString["order[0][dir]"] != null)
            {
                sortColumnDir = Request.QueryString["order[0][dir]"];
            }

            int pageSize = length;
            int skip = start;
             
            List<RCheck> checks = DataManager.GetChecks(sortColumn, sortColumnDir, skip, pageSize);

            // See https://www.codeproject.com/Tips/1011531/Using-jQuery-DataTables-with-Server-Side-Processing
            var jsonData = new
            {
                draw = draw,
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