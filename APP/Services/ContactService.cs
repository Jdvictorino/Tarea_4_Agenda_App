using Tarea_4_Agenda_App.Models;

namespace Tarea_4_Agenda_App.Services
{
    public interface IContactService
    {
        List<Contact> GetAllContacts();
        void AddContact(Contact contact);
        void DeleteContact(int id);
    }

    public class ContactService : IContactService
    {
        private static List<Contact> contacts = new List<Contact>();
        private static int nextId = 1;

        public List<Contact> GetAllContacts() => contacts;

        public void AddContact(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Nombre) || string.IsNullOrWhiteSpace(contact.Telefono))
            {
                throw new ArgumentException("Nombre y Teléfono son requeridos");
            }
            contact.Id = nextId++;
            contacts.Add(contact);
        }

        public void DeleteContact(int id)
        {
            var contact = contacts.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                contacts.Remove(contact);
            }
        }
    }
}
