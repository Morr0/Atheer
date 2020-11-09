using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace AtheerBackend.Utilities
{
    public class IPAddressCountryFinder
    {
        public static async Task<string> GetCountryByIp(string ip)
        {
            using var httpClient = new HttpClient();
            string endpoint = $"http://ip-api.com/json/{ip}";
            var response = await httpClient.GetAsync(endpoint).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync()
                    .ConfigureAwait(false)).ConfigureAwait(false);
                // Refer to https://ip-api.com/docs/api:json
                return doc.RootElement.GetProperty("country").GetString();
            }

            throw new Exception();
        }
    }
}