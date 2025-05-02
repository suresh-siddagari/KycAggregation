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

            return MapToAggregatedKyc(ssn, personalData, contactDetails, kycFormData);
        }

        private AggregatedKyc MapToAggregatedKyc(string ssn, PersonDetail personalData, ContactDetail contactDetails, KycForm kycFormData)
        {
            var address = contactDetails.Address?.FirstOrDefault()?.FullAddress ?? string.Empty;
            var phoneNumber = contactDetails.PhoneNumbers?.FirstOrDefault(x => x.Preferred)?.Number ?? string.Empty;
            var email = contactDetails.Emails?.FirstOrDefault(x => x.Preferred)?.EmailAddress ?? string.Empty;
            var taxCountry = kycFormData.Items.FirstOrDefault(x => x.Key == "tax_country")?.Value ?? string.Empty;
            var income = int.TryParse(kycFormData.Items.FirstOrDefault(x => x.Key == "annual_income")?.Value, out var incomeValue) ? incomeValue : 0;

            return new AggregatedKyc
            {
                Ssn = ssn,
                FirstName = personalData.FirstName,
                LastName = personalData.LastName,
                Address = address,
                PhoneNumber = phoneNumber,
                Email = email,
                TaxCountry = taxCountry,
                Income = income
            };
        }

        // get personal data of a customer
        public async Task<PersonDetail?> GetPersonalData(string ssn)
        {
            return await FetchData<PersonDetail>($"{customerdataurl}/personal-details/{ssn}");
        }

        //get contact details of a customer
        public async Task<ContactDetail?> GetContactDetails(string ssn)
        {
            return await FetchData<ContactDetail>($"{customerdataurl}/contact-details/{ssn}");
        }

        // get kyc form data of a customer according to a specificed data
        public async Task<KycForm?> GetKycFormData(string ssn, string asOfDate)
        {
            return await FetchData<KycForm>($"{customerdataurl}/kyc-form/{ssn}/{asOfDate}");
        }

        private async Task<T?> FetchData<T>(string url) where T : class
        {
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>()
                : null;
        }
    }
}