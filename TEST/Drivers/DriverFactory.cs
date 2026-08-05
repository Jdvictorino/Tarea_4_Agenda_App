using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AgendaPruebasSelenium.Drivers
{
    public static class DriverFactory
    {
        public static IWebDriver CreateDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            // options.AddArgument("--headless=new"); // NO actives esto al grabar el video

            IWebDriver driver = new ChromeDriver(options);
            return driver;
        }
    }
}
