using System.Text.Json.Serialization;

namespace KycApi.Model
{
    public class PhoneNumber
    {
        [JsonPropertyName("preferred")]
        public bool Preferred { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; } = "";
    }
}