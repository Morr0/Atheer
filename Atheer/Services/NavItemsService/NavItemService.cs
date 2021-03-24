using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Models;

namespace Atheer.Services.NavItemsService
{
    public class NavItemService : INavItemsService
    {
        private static readonly List<NavItem> NavItems = new()
        {
            new NavItem
            {
                Name = "Hello",
                Url = "https://github.com/morr0"
            }
        };
        
        
        public Task Add(string name, string url)
        {
            var item = new NavItem
            {
                Name = name,
                Url = url
            };
            
            lock (NavItems)
            {
                NavItems.Add(item);
            }
            
            return Task.CompletedTask;
        }

        public IList<NavItem> Get()
        {
            lock (NavItems)
            {
                return NavItems;
            }
        }
    }
}