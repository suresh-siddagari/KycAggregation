using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KycApi.Model
{
    public class Address
    {
        [JsonPropertyName("street")]
        public string Street { get; set; } = "";

        [JsonPropertyName("city")]
        public string City { get; set; } = "";

        [JsonPropertyName("state")]
        public string State { get; set; } = "";

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = "";

        [JsonPropertyName("country")]
        public string Country { get; set; } = "";

        public string FullAddress
        {
            get
            {
                return $"{Street}, {PostalCode},{City}";
            }
        }
    }
}