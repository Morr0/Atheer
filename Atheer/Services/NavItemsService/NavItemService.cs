using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Models;
using Atheer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Atheer.Services.NavItemsService
{
    public class NavItemService : INavItemsService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        private static List<NavItem> _navItems = new(1);

        public NavItemService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            LoadNavItemsFromDb();
        }

        private void LoadNavItemsFromDb()
        {
            Task.Run(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Data>();

                var items = context.NavItems.ToList();
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

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Data>();

            await context.AddAsync(item).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
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
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Data>();

            var item = await context.NavItems.Where(x => x.Id == id)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            if (item is null) return;

            lock (_navItems)
            {
                _navItems = _navItems.Where(x => x.Id != id).ToList();
            }

            context.Remove(item);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}