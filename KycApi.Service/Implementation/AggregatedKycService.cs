using KycApi.Model;
using KycApi.Service.Interface;
using System.Net.Http.Json;

namespace KycApi.Service.Implementation
{
    public class AggregatedKycService : IAggregatedKycService
    {
        private readonly HttpClient _httpClient;
        private const string customerdataurl = "https://8ea49aa1-2446-43f0-8da4-5100c85e931f-00-2dhb5v2olpjzv.worf.replit.dev/api/";

        public AggregatedKycService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // get aggregated kyc data of a customer
        public async Task<AggregatedKyc?> GetAggregatedKycData(string ssn)
        {
            //persona data
            var personalData = await GetPersonalData(ssn);

            //contact details
            var contactDetails = await GetContactDetails(ssn);
            //kyc form data
            var kycFormData = await GetKycFormData(ssn, DateTime.Now.ToString("yyyy-MM-dd"));

            if (personalData == null || contactDetails == null || kycFormData == null)
            {
                return null;
            }

            var address = contactDetails.Address?.FirstOrDefault();
            var phoneNumber = contactDetails.PhoneNumbers?.FirstOrDefault(x => x.Preferred);
            var email = contactDetails.Emails?.FirstOrDefault(x => x.Preferred);
            var taxCountry = kycFormData.Items.FirstOrDefault(x => x.Key == "tax_country")?.Value ?? string.Empty;
            var incomeStringValue = kycFormData.Items.FirstOrDefault(x => x.Key == "annual_income")?.Value ?? string.Empty;
            var income = int.TryParse(incomeStringValue, out var incomeValue) ? incomeValue : 0;

            //aggregated kyc
            var aggregatedKyc = new AggregatedKyc
            {
                Ssn = ssn,
                FirstName = personalData != null ? personalData.FirstName : string.Empty,
                LastName = personalData != null ? personalData.LastName : string.Empty,
                Address = address != null ? address.FullAddress : string.Empty,
                PhoneNumber = phoneNumber != null ? phoneNumber.Number : string.Empty,
                Email = email != null ? email.EmailAddress : string.Empty,
                TaxCountry = taxCountry,
                Income = income,
            };

            return aggregatedKyc;
        }

        // get personal data of a customer
        public async Task<PersonDetail?> GetPersonalData(string ssn)
        {
            var response = await _httpClient.GetAsync($"{customerdataurl}/personal-details/{ssn}");

            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<PersonDetail>()
                : null;
        }

        //get contact details of a customer
        public async Task<ContactDetail?> GetContactDetails(string ssn)
        {
            var response = await _httpClient.GetAsync($"{customerdataurl}/contact-details/{ssn}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ContactDetail>()
                : null;
        }

        // get kyc form data of a customer according to a specificed data
        public async Task<KycForm?> GetKycFormData(string ssn, string asOfDate)
        {
            var response = await _httpClient.GetAsync($"{customerdataurl}/kyc-form/{ssn}/{asOfDate}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<KycForm>()
                : null;
        }
    }
}