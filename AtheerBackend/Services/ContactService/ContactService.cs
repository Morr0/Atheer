using System;
using System.Threading.Tasks;
using AtheerBackend.Utilities;
using AtheerCore.Models.Contact;

namespace AtheerBackend.Services.ContactService
{
    public class ContactService : IContactService
    {
        public async Task Contact(Contact contact)
        {
            await PopulateCountryPropertyIfPossible(contact).ConfigureAwait(false);
            // Put to DB
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