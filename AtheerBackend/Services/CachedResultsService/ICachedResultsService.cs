using System.Threading.Tasks;
using AtheerBackend.DTOs;
using AtheerCore.Models;

namespace AtheerBackend.Services.CachedResultsService
{
    public interface ICachedResultsService
    {
        void Set(ref BlogPost post);
        BlogPost Get(ref BlogPostPrimaryKey primaryKey);
        void Invalidate(ref BlogPostPrimaryKey primaryKey);
    }
}