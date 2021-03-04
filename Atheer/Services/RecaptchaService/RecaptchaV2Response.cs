using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Atheer.Services.RecaptchaService
{
    public class RecaptchaV2Response
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}