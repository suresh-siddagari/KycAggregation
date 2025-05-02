using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KycApi.Model
{
    public class KycForm
    {
        [JsonPropertyName("items")]
        public List<Kyctem> Items { get; set; }
    }
}