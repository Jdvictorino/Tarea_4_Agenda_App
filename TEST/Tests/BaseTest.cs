using NUnit.Framework;
using OpenQA.Selenium;
using AgendaPruebasSelenium.Drivers;
using AgendaPruebasSelenium.Utils;

namespace AgendaPruebasSelenium.Tests
{
    [TestFixture]
    public abstract class BaseTest
    {
        protected IWebDriver Driver = null!;
        protected const string BaseUrl = "http://localhost:5000"; // AJUSTA a la URL real de tu Agenda de Contactos

        [SetUp]
        public void Setup()
        {
            Driver = DriverFactory.CreateDriver();
            var testName = TestContext.CurrentContext.Test.Name;
            ExtentReportManager.Test = ExtentReportManager.GetInstance().CreateTest(testName);
        }

        [TearDown]
        public void TearDown()
        {
            var estado = TestContext.CurrentContext.Result.Outcome.Status;
            var nombrePrueba = TestContext.CurrentContext.Test.Name;
            var rutaCaptura = ScreenshotHelper.Capturar(Driver, nombrePrueba);

            if (estado == NUnit.Framework.Interfaces.TestStatus.Passed)
                ExtentReportManager.Test!.Pass("Prueba exitosa").AddScreenCaptureFromPath(rutaCaptura);
            else
                ExtentReportManager.Test!.Fail("Prueba fallida: " + TestContext.CurrentContext.Result.Message)
                    .AddScreenCaptureFromPath(rutaCaptura);

            Driver?.Quit();
            Driver?.Dispose();
        }

        [OneTimeTearDown]
        public void FlushReport() => ExtentReportManager.GetInstance().Flush();
    }
}
