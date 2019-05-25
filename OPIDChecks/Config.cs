using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OPIDChecks
{
    public class Config
    {
        public static string ConnectionString
        {
            get
            {
                // The value configured on Web.config is overwritten at AppHarbor deployment time.
                //return "Data Source=DEKTOP-GDDTDIC\\SqlExpress;Initial Catalog=OpidDB;Integrated Security=True";
                return ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
            }
        }

        public static string SuperadminEmail
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SuperadminEmail"];
            }
        }

        public static string SuperadminPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SuperadminPwd"];
            }
        }
    }
}