using OpenQA.Selenium;

namespace AgendaPruebasSelenium.Pages
{
    public class LoginPage : BasePage
    {
        private readonly By _usernameInput = By.Id("username");          // AJUSTA
        private readonly By _passwordInput = By.Id("password");          // AJUSTA
        private readonly By _loginButton = By.Id("btnLogin");            // AJUSTA
        private readonly By _errorMessage = By.Id("errorMessage");       // AJUSTA

        public LoginPage(IWebDriver driver) : base(driver) { }

        public void IniciarSesion(string username, string password)
        {
            var usernameField = WaitVisible(_usernameInput);
            usernameField.Clear();
            usernameField.SendKeys(username);

            var passwordField = Driver.FindElement(_passwordInput);
            passwordField.Clear();
            passwordField.SendKeys(password);

            System.Threading.Thread.Sleep(300); // Pequeño delay para asegurar que el formulario esté listo

            WaitClickable(_loginButton).Click();
        }

        public bool HayMensajeDeError() => Driver.FindElements(_errorMessage).Count > 0;

        public string ObtenerMensajeError()
        {
            try
            {
                Wait.Until(d => d.FindElement(_errorMessage).Displayed && d.FindElement(_errorMessage).Text.Length > 0);
                return Driver.FindElement(_errorMessage).Text;
            }
            catch
            {
                return Driver.FindElement(_errorMessage).Text ?? string.Empty;
            }
        }
    }
}
