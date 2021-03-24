using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Models;

namespace Atheer.Services.NavItemsService
{
    public interface INavItemsService
    {
        Task Add(string name, string url);
        IList<NavItem> Get();
        Task Remove(int id);
    }
}