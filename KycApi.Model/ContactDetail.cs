﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KycApi.Model
{
    public class ContactDetail
    {
        [JsonPropertyName("address")]
        public List<Address> Address { get; set; } = new();

        [JsonPropertyName("emails")]
        public List<Email> Emails { get; set; } = new();

        [JsonPropertyName("phone_numbers")]
        public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    }
}