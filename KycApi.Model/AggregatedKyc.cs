using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KycApi.Model
{
    public class AggregatedKyc
    {
        [JsonPropertyName("ssn")]
        public string Ssn { get; set; } = "";

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("tax_country")]
        public string TaxCountry { get; set; }

        [JsonPropertyName("income")]
        public int? Income { get; set; }
    }
}