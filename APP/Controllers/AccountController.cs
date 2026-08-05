using Microsoft.AspNetCore.Mvc;
using Tarea_4_Agenda_App.Models;

namespace Tarea_4_Agenda_App.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginViewModel model)
        {
            // Simple authentication: admin/admin123
            if (model.Username == "admin" && model.Password == "admin123")
            {
                HttpContext.Session.SetString("username", model.Username);
                return RedirectToAction("Index", "Agenda");
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
            return View(model);
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
