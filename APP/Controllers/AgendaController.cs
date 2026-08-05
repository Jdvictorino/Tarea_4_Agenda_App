using Microsoft.AspNetCore.Mvc;
using Tarea_4_Agenda_App.Models;
using Tarea_4_Agenda_App.Services;

namespace Tarea_4_Agenda_App.Controllers
{
    [Route("agenda")]
    public class AgendaController : Controller
    {
        private readonly IContactService _contactService;

        public AgendaController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var contacts = _contactService.GetAllContacts();
            return View(contacts);
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(Contact contact)
        {
            try
            {
                _contactService.AddContact(contact);
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                var contacts = _contactService.GetAllContacts();
                ViewBag.ErrorMessage = ex.Message;
                return View("Index", contacts);
            }
        }

        [HttpPost]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            _contactService.DeleteContact(id);
            return RedirectToAction("Index");
        }
    }
}
