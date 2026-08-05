using OpenQA.Selenium;

namespace AgendaPruebasSelenium.Pages
{
    public class ContactosPage : BasePage
    {
        private readonly By _btnNuevoContacto = By.Id("btnGuardarContacto");      // AJUSTA
        private readonly By _inputNombre = By.Id("nombreContact");               // AJUSTA
        private readonly By _inputTelefono = By.Id("telefonoContact");           // AJUSTA
        private readonly By _inputCorreo = By.Id("correo");                      // AJUSTA
        private readonly By _btnGuardar = By.Id("btnGuardarContacto");           // AJUSTA
        private readonly By _tablaFilas = By.CssSelector("#tablaContactos tbody tr"); // AJUSTA
        private readonly By _inputBuscar = By.Id("buscar-contacto");             // AJUSTA
        private readonly By _mensajeExito = By.CssSelector(".toast-success, .alert-success, [role='status'].success"); // AJUSTA
        private readonly By _mensajeError = By.CssSelector("#mensajeErrorAgenda, .toast-error, .alert-error"); // AJUSTA
        private readonly By _btnConfirmarEliminar = By.Id("btnConfirmarModalEliminar");   // AJUSTA
        private readonly By _btnCancelarEliminar = By.Id("btnCancelarModalEliminar");     // AJUSTA

        public ContactosPage(IWebDriver driver) : base(driver) { }

        public void AbrirFormularioNuevoContacto()
        {
            // Si es un modal o formulario que se abre, implementar la lógica
            // Por ejemplo: hacer clic en un botón "Nuevo Contacto"
        }

        public void LlenarFormulario(string nombre, string telefono, string correo = "")
        {
            var nombreInput = WaitVisible(_inputNombre);
            nombreInput.Clear();
            nombreInput.SendKeys(nombre);

            var telefonoInput = Driver.FindElement(_inputTelefono);
            telefonoInput.Clear();
            telefonoInput.SendKeys(telefono);

            // Email field is optional and may not exist in all versions
            if (!string.IsNullOrEmpty(correo))
            {
                try
                {
                    var correoInput = Driver.FindElement(_inputCorreo);
                    correoInput.Clear();
                    correoInput.SendKeys(correo);
                }
                catch (NoSuchElementException)
                {
                    // Email field doesn't exist in this version, skip it
                }
            }
        }

        public void Guardar() => WaitClickable(_btnGuardar).Click();

        public void CrearContacto(string nombre, string telefono, string correo = "")
        {
            AbrirFormularioNuevoContacto();
            System.Threading.Thread.Sleep(500); // Esperar a que se abra el formulario
            LlenarFormulario(nombre, telefono, correo);
            Guardar();
            System.Threading.Thread.Sleep(500); // Esperar la respuesta del servidor
        }

        public void Buscar(string texto)
        {
            var input = WaitVisible(_inputBuscar);
            input.Clear();
            input.SendKeys(texto);
            System.Threading.Thread.Sleep(500); // Esperar a que se filtre
        }

        public int ContarFilasResultado() => Driver.FindElements(_tablaFilas).Count;

        public bool ExisteMensajeExito()
        {
            try
            {
                return Driver.FindElements(_mensajeExito).Count > 0 && 
                       Driver.FindElement(_mensajeExito).Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool ExisteMensajeError()
        {
            try
            {
                return Driver.FindElements(_mensajeError).Count > 0 && 
                       Driver.FindElement(_mensajeError).Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void EditarPrimeraFila() =>
            Driver.FindElement(By.CssSelector("#tablaContactos tbody tr:first-child button[class*='editar']")).Click(); // AJUSTA

        public void IniciarEliminacionPrimeraFila() =>
            Driver.FindElement(By.CssSelector("#tablaContactos tbody tr:first-child button[class*='eliminar']")).Click(); // AJUSTA

        public void ConfirmarEliminacion() => WaitClickable(_btnConfirmarEliminar).Click();

        public void CancelarEliminacion() => WaitClickable(_btnCancelarEliminar).Click();

        public void EliminarPrimeraFila()
        {
            IniciarEliminacionPrimeraFila();
            System.Threading.Thread.Sleep(300);
            if (Driver.FindElements(_btnConfirmarEliminar).Count > 0)
                ConfirmarEliminacion();
        }
    }
}
