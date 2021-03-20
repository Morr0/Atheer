using System.Threading.Tasks;

namespace Atheer.Services.OAuthService
{
    public interface IOAuthService
    {
        Task<OAuthUserInfo> GetUserInfo(OAuthProvider provider, string authCode);
    }
}