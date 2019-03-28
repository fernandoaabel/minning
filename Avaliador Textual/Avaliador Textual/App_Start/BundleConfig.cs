using System.Web.Optimization;

namespace Avaliador_Textual
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/otf").Include(
                       "~/Scripts/jquery-{version}.js",
                       "~/Scripts/jquery-ui-{version}.js",
                       "~/Scripts/jquery-unobtrusive*",
                       "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // DropZone JS
            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            // DataTable JS
            bundles.Add(new ScriptBundle("~/bundles/datatablescripts").Include(
                     "~/Scripts/DataTables/jquery.dataTables.js",
                     "~/Scripts/DataTables/dataTables.bootstrap.js"));
            // My Scripts
            bundles.Add(new ScriptBundle("~/bundles/myscripts").Include(
                     "~/Scripts/ScriptSite.js",
                     "~/Scripts/SomeeAdsRemover.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqplot").Include(
                                   "~/Scripts/jqPlot/jquery.jqplot.js",
                                   "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js",
                                   "~/Scripts/jqPlot/plugins/jqplot.meterGaugeRenderer.js",
                                   "~/Scripts/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.js",
                                   "~/Scripts/jqPlot/plugins/jqplot.canvasAxisTickRenderer.js",
                                   "~/Scripts/Chart.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));
            
            bundles.Add(new StyleBundle("~/Content/dropzonecss").Include(
                     "~/Scripts/dropzone/basic.css",
                     "~/Scripts/dropzone/dropzone.css"));

            bundles.Add(new StyleBundle("~/Content/datatablecss").Include(
                     "~/Scripts/DataTables/css/jquery.dataTables.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                          "~/Content/themes/base/jquery.ui.core.css",
                          "~/Content/themes/base/jquery.ui.resizable.css",
                          "~/Content/themes/base/jquery.ui.selectable.css",
                          "~/Content/themes/base/jquery.ui.accordion.css",
                          "~/Content/themes/base/jquery.ui.autocomplete.css",
                          "~/Content/themes/base/jquery.ui.button.css",
                          "~/Content/themes/base/jquery.ui.dialog.css",
                          "~/Content/themes/base/jquery.ui.slider.css",
                          "~/Content/themes/base/jquery.ui.tabs.css",
                          "~/Content/themes/base/jquery.ui.datepicker.css",
                          "~/Content/themes/base/jquery.ui.progressbar.css",
                          "~/Content/themes/base/jquery.ui.theme.css",
                          "~/Scripts/jqPlot/jquery.jqplot.css"));
        }
    }
}
