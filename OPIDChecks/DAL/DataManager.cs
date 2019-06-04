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
using System.Text;

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

        public static List<CheckViewModel> GetChecks()
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var pchecks = (from check in opidcontext.RChecks select check).ToList();

                List<CheckViewModel> checks = new List<CheckViewModel>();

                foreach (RCheck rc in pchecks)
                {
                    checks.Add(new CheckViewModel
                    {
                        RecordID = rc.RecordID,
                        InterviewRecordID = rc.InterviewRecordID,
                        Num = rc.Num,
                        Name = rc.Name,
                        Date = rc.Date.ToShortDateString(),
                        Service = rc.Service,
                        Disposition = rc.Disposition
                    });
                }

                return checks;
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

        public static string GetTimestamp()
        {
            // Set timestamp when resolvedController is loaded. This allows
            // the timestamp to be made part of the page title, which allows
            // the timestamp to appear in the printed file and also as part
            // of the Excel file name of both the angular datatable and
            // the importme file.

            // This compensates for the fact that DateTime.Now on the AppHarbor server returns
            // the time in the timezone of the server.
            // Here we convert UTC to Central Standard Time to get the time in Houston.
            // It also properly handles daylight savings time.
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime cst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "UTC", "Central Standard Time");
            string timestamp = cst.ToString("dd-MMM-yyyy-hhmm");

            return timestamp;
        }

        public static string GetResearchTableName()
        {
            string timestamp = GetTimestamp();
            string fname = string.Format("Research {0}", timestamp);
            return fname;
        }  

        public static string GetResearchTableCSV()
        {
            List<CheckViewModel> checks = DataManager.GetChecks();
            var csv = new StringBuilder();

            // N.B. No spaces between column names in the header row!
            string header = "Date,Record ID,Interview Record ID,Name,Check Number,Service,Disposition";
            csv.AppendLine(header);

            foreach (CheckViewModel check in checks)
            {
                string csvrow = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    check.Date,
                    check.RecordID,
                    check.InterviewRecordID,
                    string.Format("\"{0}\"", check.Name),
                    check.Num,
                    check.Service,
                    check.Disposition);

                csv.AppendLine(csvrow);
            }

            return csv.ToString();
        }

        public static void RestoreResearchTable(string rtFileName)
        {
            string pathToResearchTableFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", rtFileName));

            List<CheckViewModel> rchecks = MyExcelDataReader.GetCVMS(pathToResearchTableFile);
          
            RestoreRChecksTable(rchecks);
        }

        private static void RestoreRChecksTable(List<CheckViewModel> rChecks)
        {
            using (OpidDB opidcontext = new OpidDB())
            {
                var checks = opidcontext.RChecks;
               
                if (checks.Count() == 0) // Is the table empty for rebuild?
                {
                    foreach (CheckViewModel rc in rChecks)
                    {
                        
                        checks.Add(new RCheck
                        {
                            RecordID = rc.RecordID,
                            InterviewRecordID = rc.InterviewRecordID,
                            Num = rc.Num,
                            Name = rc.Name,
                            Date = Convert.ToDateTime(rc.Date),  
                            Service = rc.Service,
                            Disposition = rc.Disposition
                        });
                    }

                    opidcontext.SaveChanges();
                    return;
                }
            }
        }
    }
}