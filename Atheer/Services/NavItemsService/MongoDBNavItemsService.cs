using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Models;
using MongoDB.Driver;

namespace Atheer.Services.NavItemsService
{
    public class MongoDBNavItemsService : INavItemsService
    {
        private readonly IMongoClient _client;
        private static List<NavItem> _navItems = new(1);

        public MongoDBNavItemsService(IMongoClient client)
        {
            _client = client;
        }
        
        private void LoadNavItemsFromDb()
        {
            Task.Run(async () =>
            {
                var items = await (await _client.NavItem().FindAsync(Builders<NavItem>.Filter.Empty).CAF())
                    .ToListAsync().CAF();
                lock (_navItems)
                {
                    _navItems = items;
                }
            });
        }

        public async Task Add(string name, string url)
        {
            var item = new NavItem
            {
                Name = name,
                Url = url
            };
            
            lock (_navItems)
            {
                _navItems.Add(item);
            }

            await _client.NavItem().InsertOneAsync(item).CAF();
        }

        public IList<NavItem> Get()
        {
            lock (_navItems)
            {
                return _navItems;
            }
        }

        public async Task Remove(int id)
        {
            await _client.NavItem().FindOneAndDeleteAsync(x => x.Id == id).CAF();
            
            lock (_navItems)
            {
                _navItems = _navItems.Where(x => x.Id != id).ToList();
            }
        }
    }
}