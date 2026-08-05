using OpenQA.Selenium;

namespace AgendaPruebasSelenium.Utils
{
    public static class ScreenshotHelper
    {
        public static string Capturar(IWebDriver driver, string nombrePrueba)
        {
            var carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Capturas");
            Directory.CreateDirectory(carpeta);

            var archivo = Path.Combine(carpeta, $"{nombrePrueba}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(archivo);
            return archivo;
        }
    }
}
