using System.Threading.Tasks;

namespace Atheer.Services.RecaptchaService
{
    public interface IRecaptchaService
    {
        Task<bool> IsValidClient(string reCaptchaUserResponse);
    }
}