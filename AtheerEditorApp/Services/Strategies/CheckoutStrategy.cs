using Amazon.DynamoDBv2;
using AtheerCore.Models;
using System.Threading.Tasks;

namespace AtheerEditorApp.Services.Strategies
{
    internal abstract class CheckoutStrategy
    {
        protected AmazonDynamoDBClient _client;

        public CheckoutStrategy()
        {
            _client = new AmazonDynamoDBClient();
        }

        public abstract Task<bool> Checkout(BlogPost post);
    }
}
