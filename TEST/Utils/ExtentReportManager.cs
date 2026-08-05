using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace AgendaPruebasSelenium.Utils
{
    public static class ExtentReportManager
    {
        private static ExtentReports? _extent;
        public static ExtentTest? Test;

        public static ExtentReports GetInstance()
        {
            if (_extent == null)
            {
                var carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reportes");
                Directory.CreateDirectory(carpeta);
                var reportPath = Path.Combine(carpeta, "ReporteEjecucion.html");

                var spark = new ExtentSparkReporter(reportPath);
                spark.Config.DocumentTitle = "Reporte de Pruebas - Agenda de Contactos";
                spark.Config.ReportName = "Pruebas Automatizadas Selenium";

                _extent = new ExtentReports();
                _extent.AttachReporter(spark);
            }
            return _extent;
        }
    }
}
