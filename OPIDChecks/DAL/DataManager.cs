using Microsoft.AspNet.Identity.EntityFramework;
using OPIDChecks.DataContexts;
using OPIDChecks.Models;
using OPIDChecks.Utils;
using OPIDEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Linq.Dynamic;

namespace OPIDChecks.DAL
{
    public class DataManager
    {
        private static RoleViewModel RoleEntityToRoleViewModel(IdentityRole idRole)
        {
            return new RoleViewModel
            {
                Id = idRole.Id,
                Name = idRole.Name
            };
        }

        public static List<RoleViewModel> GetRoles()
        {
            using (IdentityDB identitycontext = new DataContexts.IdentityDB())
            {
                List<RoleViewModel> roles = new List<RoleViewModel>();
                List<IdentityRole> identityRoles = identitycontext.Roles.ToList();

                foreach (IdentityRole idRole in identityRoles)
                {
                    roles.Add(RoleEntityToRoleViewModel(idRole));
                }

                return roles;
            }
        }

        private static InvitationViewModel InviteToInvitationViewModel(Invitation invite)
        {
            return new InvitationViewModel
            {
                Id = invite.Id,
                Extended = invite.Extended.ToString("MM/dd/yyyy"),
                Accepted = (invite.Accepted == (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null ? string.Empty : invite.Accepted.ToString("MM/dd/yyyy")),
                UserName = invite.UserName,
                FullName = invite.FullName,
                Email = invite.Email,
                Role = invite.Role,
            };
        }

        public static string EditUser(InvitationViewModel invite)
        {
            using (OpidDB referralscontext = new OpidDB())
            {
                Invitation invitation = referralscontext.Invitations.Find(invite.Id);

                if (invitation != null && invitation.Accepted != (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null)
                {
                    return "Registered";
                }

                invitation.UserName = invite.UserName;
                invitation.Email = invite.Email;
                invitation.Role = invite.Role;

                referralscontext.SaveChanges();
                return "Success";
            }
        }

        public static Invitation AcceptedInvitation(string userName, string email)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                Invitation invite = opidcontext.Invitations.Where(i => i.UserName == userName).SingleOrDefault();

                if (invite != null && invite.Email == email)
                {
                    invite.Accepted = DateTime.Today;
                    opidcontext.SaveChanges();
                    return invite;
                }

                return null;
            }
        }

        private static string InvitationMessage(InvitationViewModel invitation)
        {
            string line1 = "You have been invited to register for use of OPIDChecks, the OPID Check Management System of Main Street Ministries.";
            string line2 = "Please go to opidchecks.apphb.com and click on the Register link in the main menu.";
            string line3 = string.Format("Fill out the registration form using <strong>{0}</strong> as your email address, <strong>{1}</strong> as your User Name and a password you can remember.", invitation.Email, invitation.UserName);
            string line4 = string.Format("Upon completing your registration you will be logged in to the OPID Check Management System in the role of <strong>{0}</strong>.", invitation.Role);
            return string.Format("<p>{0}<br/>{1}<br/>{2}<br/>{3}", line1, line2, line3, line4);
        }

        /*
        private static void SendEmailInvitation(InvitationViewModel invitation)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(invitation.Email));
            message.From = new MailAddress(ConfigurationManager.AppSettings["DonotreplyEmailAddr"]);
            message.Subject = "Invitation";
            message.Body = InvitationMessage(invitation);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["GmailUser"],
                    Password = ConfigurationManager.AppSettings["GmailUserPassword"]
                };

                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
        */

        public static void ExtendInvitation(InvitationViewModel invitation)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                Invitation invite = new Invitation
                {
                    Extended = DateTime.Today,
                    Accepted = (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null,
                    UserName = invitation.UserName,
                    FullName = invitation.FullName,
                    Email = invitation.Email,
                    Role = invitation.Role,
                };

                opidcontext.Invitations.Add(invite);
                opidcontext.SaveChanges();
            }

            //  SendEmailInvitation(invitation);
        }

        public static List<InvitationViewModel> GetUsers()
        {
            using (OpidDB opidcontext = new DataContexts.OpidDB())
            {
                List<InvitationViewModel> invitations = new List<InvitationViewModel>();
                List<Invitation> invites = opidcontext.Invitations.ToList();

                foreach (Invitation invite in invites)
                {
                    invitations.Add(InviteToInvitationViewModel(invite));
                }

                return invitations;
            }
        }

        /*
        public static List<CheckViewModel> GetChecks()
        {
            List<CheckViewModel> processedChecks = new List<CheckViewModel>();

            using (OpidDB opidcontext = new OpidDB())
            {
                List<RCheck> pchecks = opidcontext.RChecks.Select(u => u).ToList();

                foreach (var pcheck in pchecks)
                {
                    processedChecks.Add(new CheckViewModel
                    {
                        RecordID = pcheck.RecordID,
                        InterviewRecordID = pcheck.InterviewRecordID,
                        Num = pcheck.Num,
                        Name = pcheck.Name,
                        Date = pcheck.Date.ToShortDateString(),
                        Service = pcheck.Service,
                        Disposition = pcheck.Disposition
                    });
                }
            }

            return processedChecks;
        }
        */

        public static List<RCheck> GetChecks(int sortColumn, string sortColumnDir, int skip, int pageSize)
        {
            // Try this for server side processing
            //  https://stackoverflow.com/questions/3193930/using-jquery-datatable-for-server-side-processing-with-paging-filtering-and-sea

            using (OpidDB opidcontext = new OpidDB())
            {
                var pchecks = (from check in opidcontext.RChecks select check);

                
                //SORT
                if (sortColumn != -1 && !string.IsNullOrEmpty(sortColumnDir))
                {
                    pchecks = pchecks.OrderBy(sortColumn + " " + sortColumnDir);
                }
                
                return pchecks.Skip(skip).Take(pageSize).ToList(); ;
            }
        }

        public static bool ResearchTableIsEmpty()
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var checks = opidcontext.RChecks;

                if (checks.Count() == 0) // Is the table empty for a restore operation?
                {
                    return true;
                }
            }

            return false;
        }

        public static List<string> UploadFile(HttpPostedFileBase postedFile)
        {
            List<string> docfiles = new List<string>();
            string filePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", postedFile.FileName));
            postedFile.SaveAs(filePath);

            docfiles.Add(filePath);

            return docfiles;
        }

        public static void RestoreResearchTable(string rtFileName)
        {
            string pathToResearchTableFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", rtFileName));

            List<RCheck> rchecks = MyExcelDataReader.GetRChecks(pathToResearchTableFile);
            DateTime today = DateTime.Now;

            
            RestoreRChecksTable(rchecks);
        }

        private static void RestoreRChecksTable(List<RCheck> rChecks)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var checks = opidcontext.RChecks;

                if (checks.Count() == 0) // Is the table empty for rebuild?
                {
                    foreach (RCheck rc in rChecks)
                    {
                        checks.Add(rc);
                    }

                    opidcontext.SaveChanges();
                    return;
                }
            }
        }
    }
}