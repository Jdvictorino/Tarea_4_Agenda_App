using NUnit.Framework;
using AgendaPruebasSelenium.Pages;

namespace AgendaPruebasSelenium.Tests
{
    [TestFixture]
    public class LoginTests : BaseTest
    {
        [Test]
        [Description("HU1 - Camino feliz: login con credenciales válidas")]
        public void Login_CredencialesValidas_RedirigeAlDashboard()
        {
            var login = new LoginPage(Driver);
            login.IrA($"{BaseUrl}/login");
            login.IniciarSesion("admin", "admin123");

            // AJUSTA según tu aplicación real
            Assert.That(Driver.Url, Does.Contain("/agenda").Or.Contain("/dashboard"));
        }

        [Test]
        [Description("HU1 - Prueba negativa: login con contraseña incorrecta")]
        public void Login_ContrasenaIncorrecta_MuestraError()
        {
            var login = new LoginPage(Driver);
            login.IrA($"{BaseUrl}/login");
            login.IniciarSesion("admin", "ClaveIncorrecta");

            Assert.That(login.HayMensajeDeError(), Is.True);
        }

        [Test]
        [Description("HU1 - Prueba de límites: campos vacíos")]
        public void Login_CamposVacios_NoPermiteAcceso()
        {
            var login = new LoginPage(Driver);
            login.IrA($"{BaseUrl}/login");
            login.IniciarSesion("", "");

            Assert.That(Driver.Url, Does.Contain("/login"));
        }
    }
}
