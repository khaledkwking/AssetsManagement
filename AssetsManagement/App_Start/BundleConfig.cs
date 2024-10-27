using System.Web;
using System.Web.Optimization;

namespace AssetsManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/assets/vendors/general/jquery/dist/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/assets/vendors/general/jquery-validation/dist/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.



            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));




            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/assets/vendors/general/bootstrap/dist/css/bootstrap.css",
            //          "~/Content/site.css"));


            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));



            bundles.Add(new StyleBundle("~/App_Themes/ArabicTheme/css").Include(
                        "~/App_Themes/ArabicTheme/StyleSheet.css"));

            bundles.Add(new StyleBundle("~/App_Themes/EnglishTheme/css").Include(
                        "~/App_Themes/EnglishTheme/StyleSheet.css"));

            bundles.Add(new StyleBundle("~/Content/cssNew/css").Include(
                      "~/Content/cssNew/LoginSytle.css"));
        }
    }
}
