using System.Web.Optimization;

namespace OnlineRetailStore.Mvc
{
    public static class BundleConfig
    {
        // jquery-3.7.0.min.js and bootstrap.bundle.min.js are loaded directly via <script>
        // tags in _Layout.cshtml, not through bundles - only the style bundle below is
        // actually rendered (@Styles.Render in _Layout.cshtml).
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/store.css",
                      "~/Content/bazario-theme.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
