using AtheerCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtheerBackend.Services
{
    public interface IBlogRepository
    {
        Task<List<BlogPost>> Get(int amount, int lastEvaluatedKeyYear = 0, string lastEvaluatedKeyTitle = null);
    }
}
