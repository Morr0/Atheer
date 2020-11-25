using System;
using System.Threading.Tasks;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Repositories.Blog;
using AtheerBackend.Repositories.Contact;
using AtheerBackend.Services.ContactService.Exceptions;
using AtheerBackend.Utilities;

namespace AtheerBackend.Services.ContactService
{
    public class ContactService : IContactService
    {
        private BlogPostRepository _blogPostRepository;
        private ContactRepository _contactRepository;

        public ContactService(BlogPostRepository blogPostRepository, ContactRepository contactRepository)
        {
            _blogPostRepository = blogPostRepository;
            _contactRepository = contactRepository;
        }
        
        public async Task Contact(Contact contact)
        {
            if (!string.IsNullOrEmpty(contact.PostTitleShrinked)) await EnsureContactableBlogPost(contact).ConfigureAwait(false);

            await PopulateCountryPropertyIfPossible(contact).ConfigureAwait(false);
            await _contactRepository.PutContact(contact).ConfigureAwait(false);
        }

        private async Task EnsureContactableBlogPost(Contact contact)
        {
            var key = new BlogPostPrimaryKey
            {
                CreatedYear = contact.PostCreatedYear,
                TitleShrinked = contact.PostTitleShrinked
            };
            bool contactable = await _blogPostRepository.GetFlag(nameof(BlogPost.Contactable), key).ConfigureAwait(false);
            
            if (!contactable)
                throw new BlogPostDoesNotPermitContactException();
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