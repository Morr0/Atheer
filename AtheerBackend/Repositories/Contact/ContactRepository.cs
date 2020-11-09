using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;

namespace AtheerBackend.Repositories.Contact
{
    public class ContactRepository
    {
        private ConstantsLoader _loader;
        private AmazonDynamoDBClient _client;

        public ContactRepository(ConstantsLoader loader)
        {
            _loader = loader;
            _client = new AmazonDynamoDBClient();
        }

        public async Task PutContact(AtheerCore.Models.Contact.Contact contact)
        {
            var request = new PutItemRequest
            {
                TableName = _loader.ContactTableName,
                Item = DynamoToFromModelMapper<AtheerCore.Models.Contact.Contact>.Map(contact)
            };

            await _client.PutItemAsync(request).ConfigureAwait(false);
        }
    }
}