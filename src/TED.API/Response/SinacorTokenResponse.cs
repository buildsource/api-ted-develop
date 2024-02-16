using System.Text.Json.Serialization;

namespace TED.API.Response
{
    public class SinacorTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
