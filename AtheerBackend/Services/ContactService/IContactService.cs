using System.Threading.Tasks;
using AtheerBackend.Models;

namespace AtheerBackend.Services.ContactService
{
    public interface IContactService
    {
        Task Contact(Contact contact);
    }
}