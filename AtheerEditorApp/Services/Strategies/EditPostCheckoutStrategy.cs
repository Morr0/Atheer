using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;

namespace AtheerEditorApp.Services.Strategies
{
    public class EditPostCheckoutStrategy : CheckoutStrategy
    {
        public override Task<bool> Checkout(BlogPost post)
        {
            throw new System.NotImplementedException();
        }
    }
}