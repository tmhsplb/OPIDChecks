using DataTables.Mvc;
using OPIDChecks.DAL;
using OPIDChecks.Models;
using OPIDEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace OPIDChecks.Controllers
{
    public class AdminController : Controller
    {
        
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ResearchTable()
        {
            return View();
        }

        public ActionResult Resolved()
        {
            return View();
        }

        public JsonResult GetResolvedChecks([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<CheckViewModel> query = DataManager.GetResolvedChecksAsQueryable();
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
}