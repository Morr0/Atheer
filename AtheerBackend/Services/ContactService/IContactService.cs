using System.Threading.Tasks;
using AtheerCore.Models.Contact;

namespace AtheerBackend.Services.ContactService
{
    public interface IContactService
    {
        Task Contact(Contact contact);
    }
}