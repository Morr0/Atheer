using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.Extensions;
using AtheerBackend.Utilities;
using AtheerBackend.Models;

namespace AtheerBackend.Repositories.Contact
{
    public class ContactRepository
    {
        private AtheerConfig _loader;
        private AmazonDynamoDBClient _client;

        public ContactRepository(AtheerConfig loader)
        {
            _loader = loader;
            _client = new AmazonDynamoDBClient();
        }

        public async Task PutContact(Models.Contact contact)
        {
            var request = new PutItemRequest
            {
                TableName = _loader.ContactsTable,
                Item = DynamoToFromModelMapper<Models.Contact>.Map(contact),
                
            };

            await _client.PutItemAsync(request).ConfigureAwait(false);
        }
    }
}