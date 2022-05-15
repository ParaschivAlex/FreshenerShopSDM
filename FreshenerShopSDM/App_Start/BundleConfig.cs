using System.Web;
using System.Web.Optimization;

namespace FreshenerShopSDM
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.min.js",
                        "~/Scripts/jquery-{version}.slim.min.js",
                        "~/Scripts/jquery-{version}.slim.js",
                        "~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/respond.js",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css",
                        "~/Content/font-awesome.css"));
        }
	}
}