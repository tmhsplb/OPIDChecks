using OPIDChecks.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace OPIDChecks.DAL
{
    public class FileDownloader
    {
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
        public static void DownloadResearchTable()
        {
            List<CheckViewModel> checks = DataManager.GetChecks();
           
           
            if (checks != null)
            {
                string timestamp = GetTimestamp();
                string researchTableFileName = string.Format("Research {0}", timestamp);
                string pathToResearchTableFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Downloads/{0}.csv", researchTableFileName));

                PrepareResearchTableDownload(pathToResearchTableFile, checks);
                DownloadResearchTable(researchTableFileName, pathToResearchTableFile);
            }
        }

        private static HttpResponseMessage DownloadResearchTable(string fname, string filePath)
        {
            Byte[] bytes = null;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            System.IO.MemoryStream stream = new MemoryStream(bytes);
            result.Content = new StreamContent(stream);
            // result.Content.Headers.ContentType = new MediaTypeHeaderValue(fileType);
            //  result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/force-download");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fname
            };
            return (result);
        }

        public static void PrepareResearchTableDownload(string pathToResearchTableFile, List<CheckViewModel> checks)
        {
            var csv = new StringBuilder();
          
            string header = "Date, Record ID, Interview Record ID, Name, Check Number, Service, Disposition";
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

            File.WriteAllText(pathToResearchTableFile, csv.ToString());
        }
    }
}