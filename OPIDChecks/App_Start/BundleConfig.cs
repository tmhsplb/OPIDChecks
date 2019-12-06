using System.Web;
using System.Web.Optimization;

namespace OPIDChecks
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // Not used
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                       "~/Scripts/DataTables/jquery.dataTables.min.js"));

            // jquery mvc5 datatables js files
            bundles.Add(new ScriptBundle("~/bundles/mvc5datatables").Include(
                         "~/Scripts/DataTables/jquery.dataTables.min.js",
                         "~/Scripts/DataTables/dataTables.bootstrap.js",
                         "~/Scripts/moment.js"));

            // jquery mvc5 datatables css file
            bundles.Add(new StyleBundle("~/Content/DataTables").Include(
                       "~/Content/DataTables/css/dataTables.bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/DataTables/css").Include(
                         "~/Content/DataTables/css/jquery.dataTables.min.css"));
        }
    }
}
