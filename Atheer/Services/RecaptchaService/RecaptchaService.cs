using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;

namespace Atheer.Services.RecaptchaService
{
    // SPECS:
    // Will use reCaptcha v2 invisible: https://developers.google.com/recaptcha/docs/display
    public class RecaptchaService : IRecaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly Recaptcha _recaptcha;

        private static string RecaptchaVerificationUrl = "https://www.google.com/recaptcha/api/siteverify"; 

        public RecaptchaService(IOptions<Recaptcha> recaptcha, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _recaptcha = recaptcha.Value;
        }
        
        public async Task<bool> IsValidClient(string reCaptchaUserResponse)
        {
            // refer to https://developers.google.com/recaptcha/docs/verify
            string url = $"{RecaptchaVerificationUrl}?secret={_recaptcha.SecretKey}&response={reCaptchaUserResponse}";
            
            var response = await _httpClient.PostAsync(url, new StringContent(string.Empty), CancellationToken.None).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var responseModel = await JsonSerializer.DeserializeAsync<RecaptchaV2Response>(stream)
                .ConfigureAwait(false);

            return responseModel.Success;
        }
    }
}