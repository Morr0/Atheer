using System;
using System.Threading.Tasks;
using AtheerBackend.Repositories.Contact;
using AtheerBackend.Utilities;
using AtheerCore.Models.Contact;

namespace AtheerBackend.Services.ContactService
{
    public class ContactService : IContactService
    {
        private ContactRepository _repository;

        public ContactService(ContactRepository repository)
        {
            _repository = repository;
        }
        
        public async Task Contact(Contact contact)
        {
            await PopulateCountryPropertyIfPossible(contact).ConfigureAwait(false);
            await _repository.PutContact(contact).ConfigureAwait(false);
        }

        private async Task PopulateCountryPropertyIfPossible(Contact contact)
        {
            if (!string.IsNullOrEmpty(contact.IPAddressWhenContacted))
            {
                try
                {
                    contact.CountryWhenContacted =
                        await IPAddressCountryFinder.GetCountryByIp(contact.IPAddressWhenContacted)
                            .ConfigureAwait(false);
                }
                catch (Exception){}
            }
        }
    }
}