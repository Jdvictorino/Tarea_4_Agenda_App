using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AgendaPruebasSelenium.Pages
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void IrA(string url) => Driver.Navigate().GoToUrl(url);

        protected IWebElement WaitVisible(By locator)
        {
            return Wait.Until(d =>
            {
                var el = d.FindElement(locator);
                return el.Displayed ? el : null;
            });
        }

        protected IWebElement WaitClickable(By locator)
        {
            return Wait.Until(d =>
            {
                var el = d.FindElement(locator);
                return (el.Displayed && el.Enabled) ? el : null;
            });
        }
    }
}
