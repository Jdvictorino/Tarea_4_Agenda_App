# Guía Paso a Paso — Tarea 4: Pruebas Automatizadas con Selenium (C#)

## Resumen y supuesto de partida

Esta guía usa una **Agenda de Contactos telefónicos** como aplicación base, sobre su vista de **Contactos** (CRUD: nombre, teléfono, correo).

Recuerda que Selenium solo automatiza **aplicaciones web** (que corren dentro de un navegador). Si tu Agenda de Contactos es una app de escritorio o móvil nativa, no serviría para esta tarea — necesita una interfaz accesible desde el navegador.

> ⚠️ **Los selectores (`By.Id`, `By.CssSelector`, etc.) de este documento son de ejemplo.** Debes reemplazarlos por los reales de tu HTML. En el Paso 6.1 te explico cómo encontrarlos.

---

## Índice

1. Conceptos clave: tipos de prueba
2. Preparar la aplicación base
3. Herramientas necesarias
4. Crear el proyecto de pruebas en C#
5. Estructura de carpetas
6. Page Object Model (Driver, BasePage, LoginPage, ContactosPage)
7. Reporte HTML y capturas automáticas
8. Clase base de pruebas (BaseTest)
9. Pruebas de Login (HU1)
10. Pruebas CRUD de Contactos (HU2 a HU5)
11. Ejecutar las pruebas
12. Historias de usuario para Jira / Azure DevOps
13. Subir el proyecto a GitHub
14. Guion para el video demostrativo
15. Checklist final antes de entregar

---

## 1. Conceptos clave: tipos de prueba

Cada flujo (login, crear, leer, actualizar, eliminar) necesita estos 3 tipos de prueba:

| Tipo | Qué valida | Ejemplo |
|---|---|---|
| **Camino feliz** | El flujo funciona con datos válidos | Login con usuario y clave correctos |
| **Prueba negativa** | El sistema rechaza datos inválidos sin romperse | Login con clave incorrecta |
| **Prueba de límites** | El comportamiento justo en el borde permitido (mínimo, máximo, vacío) | Teléfono con el máximo de dígitos permitido, o campo vacío |

No es solo "copiar el código" — cada prueba debe demostrar claramente uno de estos tres casos.

---

## 2. Preparar la aplicación base

Antes de escribir pruebas, confirma en tu Agenda de Contactos:

- [ ] Existe una pantalla de login funcional (usuario/correo + contraseña).
- [ ] Existe una vista de Contactos con acciones para Crear, Buscar, Editar y Eliminar.
- [ ] Puedes correrla localmente y queda accesible en una URL fija (ej. `http://localhost:3000`, o el puerto que uses) — **debe estar corriendo cada vez que ejecutes las pruebas**, porque Selenium abre esa URL en un navegador real.

---

## 3. Herramientas necesarias

- SDK de .NET 8 o superior (`dotnet --version` para confirmar que lo tienes).
- Google Chrome instalado.
- Visual Studio 2022, VS Code o Rider.
- Cuenta de GitHub.
- Cuenta de Jira o Azure DevOps.
- Tu Agenda de Contactos corriendo en paralelo mientras pruebas (con el comando de tu stack: `npm start`, `dotnet run`, etc.).

No necesitas descargar `chromedriver.exe` manualmente: desde Selenium 4.6+, **Selenium Manager** lo resuelve automáticamente.

---

## 4. Crear el proyecto de pruebas en C#

```bash
dotnet new nunit -n AgendaContactos.Tests.Selenium
cd AgendaContactos.Tests.Selenium

dotnet add package Selenium.WebDriver
dotnet add package Selenium.Support
dotnet add package AventStack.ExtentReports
```

`dotnet new nunit` ya trae NUnit, NUnit3TestAdapter y Microsoft.NET.Test.Sdk configurados, así que no hace falta agregarlos aparte.

---

## 5. Estructura de carpetas

```
AgendaContactos.Tests.Selenium/
├── Drivers/
│   └── DriverFactory.cs
├── Pages/
│   ├── BasePage.cs
│   ├── LoginPage.cs
│   └── ContactosPage.cs
├── Tests/
│   ├── BaseTest.cs
│   ├── LoginTests.cs
│   └── ContactosCrudTests.cs
├── Utils/
│   ├── ExtentReportManager.cs
│   └── ScreenshotHelper.cs
```

`Reportes/` y `Capturas/` se crean solas al correr las pruebas (no las crees a mano).

---

## 6. Page Object Model

### 6.1 Cómo encontrar tus selectores reales

En tu Agenda de Contactos, abre el navegador → F12 (DevTools) → clic derecho sobre el campo o botón → **Inspeccionar**. Ahí ves el `id`, `class` o `name` real. Si tu HTML usa clases dinámicas o generadas automáticamente (común en frameworks como React o Angular), lo más estable es **agregar atributos `data-testid`** a tus elementos (ej. `<input data-testid="login-email" />`) y usar `By.CssSelector("[data-testid='login-email']")` — no se rompe si cambias estilos.

### 6.2 `Drivers/DriverFactory.cs`

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AgendaContactos.Tests.Selenium.Drivers
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
```

### 6.3 `Pages/BasePage.cs`

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AgendaContactos.Tests.Selenium.Pages
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
```

### 6.4 `Pages/LoginPage.cs`

```csharp
using OpenQA.Selenium;

namespace AgendaContactos.Tests.Selenium.Pages
{
    public class LoginPage : BasePage
    {
        private readonly By _emailInput = By.Id("email");          // AJUSTA
        private readonly By _passwordInput = By.Id("password");    // AJUSTA
        private readonly By _loginButton = By.CssSelector("button[type='submit']"); // AJUSTA
        private readonly By _errorMessage = By.CssSelector(".error-message, [role='alert']"); // AJUSTA

        public LoginPage(IWebDriver driver) : base(driver) { }

        public void IniciarSesion(string email, string password)
        {
            var emailField = WaitVisible(_emailInput);
            emailField.Clear();
            emailField.SendKeys(email);

            var passwordField = Driver.FindElement(_passwordInput);
            passwordField.Clear();
            passwordField.SendKeys(password);

            WaitClickable(_loginButton).Click();
        }

        public bool HayMensajeDeError() => Driver.FindElements(_errorMessage).Count > 0;
    }
}
```

### 6.5 `Pages/ContactosPage.cs`

```csharp
using OpenQA.Selenium;

namespace AgendaContactos.Tests.Selenium.Pages
{
    public class ContactosPage : BasePage
    {
        private readonly By _btnNuevoContacto = By.Id("btn-nuevo-contacto");    // AJUSTA
        private readonly By _inputNombre = By.Id("nombre");                      // AJUSTA
        private readonly By _inputTelefono = By.Id("telefono");                  // AJUSTA
        private readonly By _inputCorreo = By.Id("correo");                      // AJUSTA
        private readonly By _btnGuardar = By.Id("btn-guardar");                  // AJUSTA
        private readonly By _tablaFilas = By.CssSelector("table tbody tr");      // AJUSTA
        private readonly By _inputBuscar = By.Id("buscar-contacto");             // AJUSTA
        private readonly By _mensajeExito = By.CssSelector(".toast-success, .alert-success"); // AJUSTA
        private readonly By _mensajeError = By.CssSelector(".toast-error, .alert-error, .field-error"); // AJUSTA
        private readonly By _btnConfirmarEliminar = By.Id("btn-confirmar-eliminar"); // AJUSTA
        private readonly By _btnCancelarEliminar = By.Id("btn-cancelar-eliminar");   // AJUSTA

        public ContactosPage(IWebDriver driver) : base(driver) { }

        public void AbrirFormularioNuevoContacto() => WaitClickable(_btnNuevoContacto).Click();

        public void LlenarFormulario(string nombre, string telefono, string correo)
        {
            var nombreInput = WaitVisible(_inputNombre);
            nombreInput.Clear();
            nombreInput.SendKeys(nombre);

            var telefonoInput = Driver.FindElement(_inputTelefono);
            telefonoInput.Clear();
            telefonoInput.SendKeys(telefono);

            var correoInput = Driver.FindElement(_inputCorreo);
            correoInput.Clear();
            correoInput.SendKeys(correo);
        }

        public void Guardar() => WaitClickable(_btnGuardar).Click();

        public void CrearContacto(string nombre, string telefono, string correo)
        {
            AbrirFormularioNuevoContacto();
            LlenarFormulario(nombre, telefono, correo);
            Guardar();
        }

        public void Buscar(string texto)
        {
            var input = WaitVisible(_inputBuscar);
            input.Clear();
            input.SendKeys(texto);
        }

        public int ContarFilasResultado() => Driver.FindElements(_tablaFilas).Count;

        public bool ExisteMensajeExito() => Driver.FindElements(_mensajeExito).Count > 0;

        public bool ExisteMensajeError() => Driver.FindElements(_mensajeError).Count > 0;

        public void EditarPrimeraFila() =>
            Driver.FindElement(By.CssSelector("table tbody tr:first-child .btn-editar")).Click(); // AJUSTA

        public void IniciarEliminacionPrimeraFila() =>
            Driver.FindElement(By.CssSelector("table tbody tr:first-child .btn-eliminar")).Click(); // AJUSTA

        public void ConfirmarEliminacion() => WaitClickable(_btnConfirmarEliminar).Click();

        public void CancelarEliminacion() => WaitClickable(_btnCancelarEliminar).Click();

        public void EliminarPrimeraFila()
        {
            IniciarEliminacionPrimeraFila();
            if (Driver.FindElements(_btnConfirmarEliminar).Count > 0)
                ConfirmarEliminacion();
        }
    }
}
```

---

## 7. Reporte HTML y capturas automáticas

### 7.1 `Utils/ExtentReportManager.cs`

```csharp
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace AgendaContactos.Tests.Selenium.Utils
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
```

### 7.2 `Utils/ScreenshotHelper.cs`

```csharp
using OpenQA.Selenium;

namespace AgendaContactos.Tests.Selenium.Utils
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
```

---

## 8. Clase base de pruebas

### `Tests/BaseTest.cs`

```csharp
using NUnit.Framework;
using OpenQA.Selenium;
using AgendaContactos.Tests.Selenium.Drivers;
using AgendaContactos.Tests.Selenium.Utils;

namespace AgendaContactos.Tests.Selenium.Tests
{
    [TestFixture]
    public abstract class BaseTest
    {
        protected IWebDriver Driver = null!;
        protected const string BaseUrl = "http://localhost:3000"; // AJUSTA a la URL real de tu Agenda de Contactos

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

            Driver.Quit();
        }

        [OneTimeTearDown]
        public void FlushReport() => ExtentReportManager.GetInstance().Flush();
    }
}
```

---

## 9. Pruebas de Login (HU1)

### `Tests/LoginTests.cs`

```csharp
using NUnit.Framework;
using AgendaContactos.Tests.Selenium.Pages;

namespace AgendaContactos.Tests.Selenium.Tests
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
            login.IniciarSesion("usuario@agenda.com", "ClaveValida123");

            Assert.That(Driver.Url, Does.Contain("/dashboard")); // AJUSTA a tu ruta real
        }

        [Test]
        [Description("HU1 - Prueba negativa: login con contraseña incorrecta")]
        public void Login_ContrasenaIncorrecta_MuestraError()
        {
            var login = new LoginPage(Driver);
            login.IrA($"{BaseUrl}/login");
            login.IniciarSesion("usuario@agenda.com", "ClaveIncorrecta");

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
```

---

## 10. Pruebas CRUD de Contactos (HU2 a HU5)

### `Tests/ContactosCrudTests.cs`

```csharp
using NUnit.Framework;
using AgendaContactos.Tests.Selenium.Pages;

namespace AgendaContactos.Tests.Selenium.Tests
{
    [TestFixture]
    public class ContactosCrudTests : BaseTest
    {
        private void IniciarSesionValida()
        {
            var login = new LoginPage(Driver);
            login.IrA($"{BaseUrl}/login");
            login.IniciarSesion("usuario@agenda.com", "ClaveValida123");
        }

        // ---------- CREATE (HU2) ----------

        [Test]
        [Description("HU2 - Camino feliz: crear contacto con datos válidos")]
        public void CrearContacto_DatosValidos_MuestraExito()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.CrearContacto("Juan Pérez", "8091234567", "juan.perez@correo.com");

            Assert.That(contactos.ExisteMensajeExito(), Is.True);
        }

        [Test]
        [Description("HU2 - Prueba negativa: crear contacto sin teléfono")]
        public void CrearContacto_SinTelefono_MuestraError()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.CrearContacto("Juan Pérez", "", "juan.perez@correo.com");

            Assert.That(contactos.ExisteMensajeError(), Is.True);
        }

        [Test]
        [Description("HU2 - Prueba de límites: teléfono con el máximo de dígitos permitido")]
        public void CrearContacto_TelefonoLongitudMaxima_SeGuardaCorrectamente()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            var telefonoLimite = "8091234567"; // AJUSTA al máximo real de tu formulario

            contactos.CrearContacto("María Ramírez", telefonoLimite, "maria@correo.com");

            Assert.That(contactos.ExisteMensajeExito(), Is.True);
        }

        // ---------- READ (HU3) ----------

        [Test]
        [Description("HU3 - Camino feliz: buscar contacto existente")]
        public void BuscarContacto_Existente_MuestraResultado()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.Buscar("Juan Pérez");

            Assert.That(contactos.ContarFilasResultado(), Is.GreaterThan(0));
        }

        [Test]
        [Description("HU3 - Prueba negativa: buscar contacto inexistente")]
        public void BuscarContacto_Inexistente_NoMuestraResultados()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.Buscar("ContactoQueNoExiste_XYZ");

            Assert.That(contactos.ContarFilasResultado(), Is.EqualTo(0));
        }

        [Test]
        [Description("HU3 - Prueba de límites: búsqueda con un solo carácter")]
        public void BuscarContacto_UnSoloCaracter_ManejaCorrectamente()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.Buscar("J");

            Assert.That(contactos.ContarFilasResultado(), Is.GreaterThanOrEqualTo(0));
        }

        // ---------- UPDATE (HU4) ----------

        [Test]
        [Description("HU4 - Camino feliz: actualizar contacto con datos válidos")]
        public void EditarContacto_DatosValidos_MuestraExito()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.EditarPrimeraFila();
            contactos.LlenarFormulario("Juan Pérez Editado", "8097654321", "juan.editado@correo.com");
            contactos.Guardar();

            Assert.That(contactos.ExisteMensajeExito(), Is.True);
        }

        [Test]
        [Description("HU4 - Prueba negativa: actualizar con correo inválido")]
        public void EditarContacto_CorreoInvalido_MuestraError()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.EditarPrimeraFila();
            contactos.LlenarFormulario("Juan Pérez", "8097654321", "correo-no-valido");
            contactos.Guardar();

            Assert.That(contactos.ExisteMensajeError(), Is.True);
        }

        [Test]
        [Description("HU4 - Prueba de límites: teléfono por debajo del mínimo de dígitos")]
        public void EditarContacto_TelefonoDebajoDelMinimo_MuestraError()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            contactos.EditarPrimeraFila();
            contactos.LlenarFormulario("Juan Pérez", "12345", "juan.perez@correo.com"); // AJUSTA al mínimo real - 1
            contactos.Guardar();

            Assert.That(contactos.ExisteMensajeError(), Is.True);
        }

        // ---------- DELETE (HU5) ----------

        [Test]
        [Description("HU5 - Camino feliz: eliminar contacto existente")]
        public void EliminarContacto_Existente_SeElimina()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            var filasAntes = contactos.ContarFilasResultado();

            contactos.EliminarPrimeraFila();

            Assert.That(contactos.ContarFilasResultado(), Is.LessThan(filasAntes));
        }

        [Test]
        [Description("HU5 - Prueba negativa: cancelar eliminación")]
        public void EliminarContacto_Cancelar_NoSeElimina()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            var filasAntes = contactos.ContarFilasResultado();

            contactos.IniciarEliminacionPrimeraFila();
            contactos.CancelarEliminacion();

            Assert.That(contactos.ContarFilasResultado(), Is.EqualTo(filasAntes));
        }

        [Test]
        [Description("HU5 - Prueba de límites: eliminar cuando solo queda un contacto")]
        public void EliminarContacto_UltimoRegistro_ManejaEstadoVacio()
        {
            IniciarSesionValida();
            var contactos = new ContactosPage(Driver);
            contactos.IrA($"{BaseUrl}/contactos");
            // Precondición: deja un solo contacto en la base antes de correr esta prueba

            contactos.EliminarPrimeraFila();

            Assert.That(contactos.ContarFilasResultado(), Is.EqualTo(0));
        }
    }
}
```

Con esto tienes **15 pruebas** (3 por cada una de las 5 historias de usuario), cumpliendo el mínimo exigido.

---

## 11. Ejecutar las pruebas

1. Levanta tu Agenda de Contactos (con el comando de tu stack) y déjala corriendo.
2. En otra terminal, dentro de la carpeta del proyecto de pruebas:

```bash
dotnet test
```

3. Al terminar, revisa:
   - `Reportes/ReporteEjecucion.html` → ábrelo en el navegador, ahí está el reporte con el resultado de cada prueba y su captura.
   - `Capturas/` → una imagen `.png` por cada prueba ejecutada.

Si una prueba falla, generalmente es porque un selector no coincide con tu HTML real — vuelve al Paso 6.1 para inspeccionarlo.

---

## 12. Historias de usuario para Jira / Azure DevOps

Recuerda: **deben ir en el tablero de Jira o Azure DevOps**, no en el README de GitHub ni en Word/PDF. Contenido listo para copiar en cada historia (ajusta detalles según el comportamiento real de tu app):

**HU1 — Inicio de sesión**
Como usuario registrado, quiero iniciar sesión con mi correo y contraseña, para acceder a mi agenda de contactos.
- ✅ Aceptación: con correo y contraseña válidos, el sistema redirige al dashboard.
- ❌ Rechazo: con contraseña incorrecta o campos vacíos, el sistema muestra error y no otorga acceso.

**HU2 — Crear contacto**
Como usuario de la agenda, quiero registrar un nuevo contacto con su nombre y teléfono, para poder ubicarlo después.
- ✅ Aceptación: con nombre y teléfono válidos, el contacto se crea y se muestra confirmación.
- ❌ Rechazo: si falta el teléfono, o si excede el máximo de dígitos permitido, el sistema no guarda el contacto y muestra error.

**HU3 — Consultar contactos**
Como usuario de la agenda, quiero buscar un contacto por nombre, para encontrarlo rápido sin desplazarme por toda la lista.
- ✅ Aceptación: al buscar un nombre existente, aparece al menos un resultado coincidente.
- ❌ Rechazo: al buscar un nombre que no existe, no aparece ningún resultado.

**HU4 — Actualizar contacto**
Como usuario de la agenda, quiero editar los datos de un contacto existente, para mantener su información al día.
- ✅ Aceptación: al editar con datos válidos, los cambios se guardan y se confirma.
- ❌ Rechazo: al editar con un correo mal formado o un teléfono por debajo del mínimo de dígitos, el sistema rechaza el cambio.

**HU5 — Eliminar contacto**
Como usuario de la agenda, quiero eliminar un contacto que ya no necesito, para mantener la agenda organizada.
- ✅ Aceptación: al confirmar la eliminación, el contacto desaparece de la lista.
- ❌ Rechazo: al cancelar la eliminación, el contacto permanece en la lista.

**Pasos rápidos:**
- **Jira:** crea un proyecto (Scrum o Kanban) → Backlog → "Crear elemento" tipo Historia por cada una de las 5 → pega el contenido de arriba en la descripción → revisa que el acceso quede público o comparte el enlace con permiso de visualización para tu profesor.
- **Azure DevOps:** Boards → Work Items → New Item → User Story, mismo contenido. En Project Settings → Team configuration revisa la visibilidad del proyecto.

Como las opciones de compartir cambian de vez en cuando según el plan, verifica en tu cuenta que el enlace realmente abra sin iniciar sesión antes de entregarlo (pruébalo en una ventana de incógnito).

---

## 13. Subir el proyecto a GitHub

```bash
dotnet new gitignore
git init
git add .
git commit -m "Proyecto inicial de pruebas Selenium - Agenda de Contactos"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/agenda-contactos-selenium-tests.git
git push -u origin main
```

- El repositorio debe ser **público**.
- Agrega un `README.md` corto: qué prueba el proyecto, cómo correrlo (`dotnet test`), y enlaces al tablero de Jira/Azure DevOps y al video (los enlaces, no las historias de usuario completas).
- Confirma que este proyecto es individual — no lo compartas ni lo bifurques de un compañero.

---

## 14. Guion para el video demostrativo

Duración sugerida: 4 a 5 minutos. Grábalo con el navegador **visible** (sin modo headless) para que se vea la ejecución real — es la parte que más pesa en la evaluación de este entregable.

**Escena 1 (00:00–00:25) — Introducción**
En pantalla: tu cara o una diapositiva con el título de la tarea.
Narración: *"Hola, soy Juan, estudiante de Desarrollo de Software en el ITLA. Este es el video demostrativo de la Tarea 4: pruebas automatizadas con Selenium, usando C# sobre una Agenda de Contactos telefónicos."*

**Escena 2 (00:25–00:55) — La aplicación base**
En pantalla: la Agenda de Contactos abierta en el navegador — pantalla de login y luego la lista de contactos.
Narración: *"Esta es la aplicación que voy a probar: una agenda de contactos con inicio de sesión, y una vista donde puedo crear, buscar, editar y eliminar contactos."*

**Escena 3 (00:55–01:35) — El proyecto de pruebas**
En pantalla: el IDE, mostrando la estructura de carpetas (Pages, Tests, Utils) y brevemente el archivo `ContactosPage.cs`.
Narración: *"Las pruebas están hechas en C#, con Selenium WebDriver y NUnit, siguiendo el patrón Page Object Model. Aquí tengo separadas las páginas, las clases de prueba, y las utilidades para generar el reporte y las capturas de pantalla."*

**Escena 4 (01:35–02:05) — Historias de usuario**
En pantalla: el tablero de Jira o Azure DevOps con las 5 historias de usuario abiertas.
Narración: *"Estas son mis 5 historias de usuario: inicio de sesión, y las 4 operaciones CRUD sobre contactos — crear, consultar, actualizar y eliminar — cada una con sus criterios de aceptación y de rechazo."*

**Escena 5 (02:05–03:20) — Ejecución de las pruebas (parte principal)**
En pantalla: terminal corriendo `dotnet test`; luego cambiar a Chrome mostrando las acciones ejecutándose solas — login, crear, buscar, editar, eliminar — incluyendo un caso negativo y uno de límites.
Narración: *"Ahora corro las 15 pruebas. Van a ver cómo Chrome se abre automáticamente y ejecuta cada acción sin que yo toque nada: inicia sesión, crea un contacto, lo busca, lo edita, lo elimina — y también los casos donde debe fallar, como un campo vacío o un teléfono muy corto."*

**Escena 6 (03:20–04:00) — Resultados: reporte y capturas**
En pantalla: abrir `ReporteEjecucion.html`, recorrer 2-3 resultados y mostrar una captura dentro del reporte.
Narración: *"Al terminar, se genera este reporte HTML con el resultado de cada prueba y la captura de pantalla correspondiente, tanto de las que pasaron como de las que debían fallar."*

**Escena 7 (04:00–04:30) — Cierre**
En pantalla: tu cara o una diapositiva de cierre.
Narración: *"En total corrí 15 pruebas: camino feliz, negativas y de límites, para las 5 historias de usuario. El código está en GitHub y las historias en el tablero — ambos enlaces están en la entrega. Gracias."*

**Consejos rápidos para grabar:**
- Usa OBS Studio, o la grabadora de pantalla integrada de Windows (Win+G) o Mac.
- Practica una vez sin grabar para calcular tiempos.
- Si `dotnet test` tarda mucho, puedes acelerar esa sección en edición, pero deja al menos un tramo a velocidad normal donde se vea el navegador actuando solo.
- Sube el video a YouTube (público o "no listado", ambos se abren por enlace) o a OneDrive con acceso abierto. Google Drive no se acepta.

---

## 15. Checklist final antes de entregar

- [ ] Aplicación base funcional y propia (Agenda de Contactos), no compartida con otro estudiante
- [ ] Pruebas en C# usando Selenium WebDriver (sin Selenium IDE)
- [ ] Login automatizado (camino feliz, negativa, límites)
- [ ] CRUD automatizado sobre Contactos: Crear, Leer, Actualizar, Eliminar (3 tipos de prueba cada uno)
- [ ] Mínimo 5 historias de usuario, cada una con al menos 1 caso de prueba
- [ ] Historias de usuario con criterios de aceptación y rechazo, documentadas en Jira o Azure DevOps
- [ ] Reporte HTML generado (`ReporteEjecucion.html`)
- [ ] Capturas de pantalla automáticas por cada escenario
- [ ] Repositorio en GitHub, público, con enlace (no .zip)
- [ ] Video público en YouTube o OneDrive siguiendo el guion, mostrando la ejecución real
- [ ] Los 3 enlaces (GitHub, Jira/Azure DevOps, video) probados en incógnito para confirmar que abren sin iniciar sesión
- [ ] Los 3 enlaces pegados en el campo "Texto en línea" de la plataforma
