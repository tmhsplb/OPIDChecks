using OPIDChecks.DAL;
using OPIDChecks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDChecks.Controllers
{
    public class SuperadminController : UsersController
    {
        public ActionResult Home()
        {
            string workingConnectionString = string.Empty;

            ViewBag.Release = Config.Release;

            switch (Config.Release)
            {
                case "Desktop":
                    workingConnectionString = Config.WorkingDesktopConnectionString;
                    break;
                case "Staging":
                    workingConnectionString = Config.WorkingStagingConnectionString;
                    break;
                case "Production":
                    workingConnectionString = Config.WorkingProductionConnectionString;
                    break;
            }

            ViewBag.ConnectionString = Config.ConnectionString;
            ViewBag.ChangedConnectionString = (Config.ConnectionString.Equals(workingConnectionString) ? "False" : "True");

            // Log.Info("Goto Superadmin home page");
            return View();
        }

        public ActionResult ManageRoles()
        {
            return RedirectToAction("Index", "Role");
        }

        public string ExtendInvitation(InvitationViewModel invite)
        {
            if (InUse(invite.UserName))
            {
                string status = string.Format("The user name {0} is already in use. Please use a different user name.", invite.UserName);
                return status;
            }

            Identity.ExtendInvitation(invite);

            return "Success";
        }

        public string EditUser(InvitationViewModel invite)
        {
            string status = Identity.EditUser(invite);
            return status;
        }


        public JsonResult GetUsers(int page, int rows)
        {
            List<InvitationViewModel> invitations = Identity.GetUsers();

            var jsonData = new
            {
                total = 1,
                page,
                records = invitations.Count,
                rows = invitations
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageUsers()
        {
            return View("Users");
        }

        public ActionResult MistakenlyResolved()
        {
            TempData["UploadedFile"] = "";
            ViewData["MergeStatus"] = "Wait for the Merge Complete message after clicking the Merge button";
            return View("MistakenlyResolved");
        }

        [HttpPost]
        public ActionResult UploadReResolvedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("ReresolvedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "ReresolvedChecks";
                ViewData["UploadedRRCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("MistakenlyResolved", model);
            }

            ModelState.AddModelError("ReresolvedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadReresolvedVoidedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("RRVoidedChecksError", "This is not an Excel xlsx file.");
                    return View("MistakenlyResolved", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "ReresolvedVoidedChecks";
                ViewData["UploadedRRVCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("MistakenlyResolved", model);
            }

            ModelState.AddModelError("VoidedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadReresolvedClearedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("RRClearedChecksError", "This is not an Excel xlsx file.");
                    return View("MistakenlyResolved", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "ReresolvedCleardChecks";
                ViewData["UploadedRRCCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("MistakenlyResolved", model);
            }

            ModelState.AddModelError("RRClearedChecksError", "Please supply a file name.");
            return View("MistakenlyResolved", model);
        }

        [HttpPost]
        public ActionResult PerformMerge()
        {
            string uploadedFile = TempData["UploadedFile"] as string;
            string fileType = TempData["FileType"] as string;

            if (string.IsNullOrEmpty(uploadedFile))
            {
                ViewData["MergeStatus"] = "Please choose a file to merge";
                return View("MistakenlyResolved");
            }

            Merger.PerformMerge(uploadedFile, fileType);

            ViewData["MergeStatus"] = "Merge Complete";
            return View("MistakenlyResolved");
        }
    }
}