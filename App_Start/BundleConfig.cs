using System.Web.Optimization;

namespace GardenShopOnline
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new MyScriptBundle("~/bundles/customerJs").Include(
                "~/assets/customer/js/vendor/bootstrap.bundle.min.js",
                "~/assets/customer/js/vendor/jquery-3.6.0.min.js",
                "~/assets/customer/js/vendor/jquery-migrate-3.3.2.min.js",
                "~/assets/customer/js/vendor/jquery.waypoints.js",
                "~/assets/customer/js/vendor/modernizr-3.11.2.min.js",
                "~/assets/customer/js/plugins/wow.min.js",
                "~/assets/customer/js/plugins/swiper-bundle.min.js",
                "~/assets/customer/js/plugins/jquery.nice-select.js",
                "~/assets/customer/js/plugins/parallax.min.js",
                "~/assets/customer/js/plugins/jquery.magnific-popup.min.js",
                "~/assets/customer/js/plugins/tippy.min.js",
                "~/assets/customer/js/plugins/ion.rangeSlider.min.js",
                "~/assets/customer/js/plugins/mailchimp-ajax.js",
                "~/assets/customer/js/plugins/jquery.counterup.js",
                "~/assets/customer/js/vendor/sweetalert2.all.min.js",
                "~/assets/customer/js/vendor/toastr.min.js"));

            bundles.Add(new MyStyleBundle("~/Content/customerCss").Include(
                "~/assets/customer/css/bootstrap.min.css",
                "~/assets/customer/css/animate.min.css",
                "~/assets/customer/css/swiper-bundle.min.css",
                "~/assets/customer/css/nice-select.css",
                "~/assets/customer/css/magnific-popup.min.css",
                "~/assets/customer/css/ion.rangeSlider.min.css",
                "~/assets/customer/css/toastr.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/assets/customer/js/vendor/jquery.validate*"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
