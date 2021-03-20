using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Atheer.Services.OAuthService
{
    public class OAuthService : IOAuthService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OAuthService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public async Task<OAuthUserInfo> GetUserInfo(OAuthProvider provider, string authCode)
        {
            // TODO implement   
            throw new System.NotImplementedException();
        }
    }
}