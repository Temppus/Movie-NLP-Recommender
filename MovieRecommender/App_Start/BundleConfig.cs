using System.Web;
using System.Web.Optimization;

namespace MovieRecommender
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.ba-throttle-debounce.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/css/semantic").Include(
                      "~/Content/semantic.css"));

            bundles.Add(new ScriptBundle("~/scripts/semantic").Include(
                      "~/Scripts/semantic.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
