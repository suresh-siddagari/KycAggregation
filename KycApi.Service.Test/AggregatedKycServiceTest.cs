using KycApi.Model;
using KycApi.Service.Implementation;
using KycApi.Service.Interface;

namespace KycApi.Service.Test
{
    public class AggregatedKycServiceTest
    {
        private readonly IDataService _dataService;
        private readonly IAggregatedKycService _service;
        private readonly ICacheService _cacheService;
        private readonly HttpClient _httpClient;

        public AggregatedKycServiceTest()
        {
            _httpClient = new HttpClient();
            _cacheService = new CacheService();
            _dataService = new DataService(_httpClient, _cacheService);
            _service = new AggregatedKycService(_dataService);
        }

        // Test method : GetContactDetails
        // Test case 1: Valid SSN + Customer data does not exist => null
        // Test case 2: Valid SSN + Customer data exists => Contact details of the customer

        [Theory]
        [InlineData("19900101-1234", false)] // Customer data does not exist
        [InlineData("19800115-1234", true)]  // Customer data exists
        public async Task GetContactDetails_ValidSsn_ReturnsExpectedResult(string ssn, bool customerExists)
        {
            // Act
            var result = await _service.GetContactDetails(ssn);

            // Assert
            if (!customerExists)
            {
                Assert.Null(result); // should be null as customer data does not exist
            }
            else
            {
                Assert.NotNull(result); // should not be null as customer data exists
                Assert.NotEmpty(result.Address); // should have at least one address
                Assert.NotEmpty(result.Emails); // should have at least one email
                Assert.NotEmpty(result.PhoneNumbers); // should have at least one phone number
            }
        }

        //Test method : GetPersonalData
        // Test case : After fetching data from API, cached should be updated
        [Fact]
        public async Task GGetPersonalData_FetchData_ShouldUpdateCache()
        {
            //Arrange
            var remoteAPI = "https://8ea49aa1-2446-43f0-8da4-5100c85e931f-00-2dhb5v2olpjzv.worf.replit.dev/api/"; //TODO: move to config file
            var ssn = "19800115-1234";
            var personalDetailsUrl = $"{remoteAPI}/personal-details/{ssn}";

            //Act
            //check cache before fetching data
            var cachedDataBefore = _cacheService.Get<PersonDetail>(personalDetailsUrl.GetHashCode().ToString());
            var result = await _service.GetPersonalData(ssn);
            //check cache after fetching data
            var cachedDataAfter = _cacheService.Get<PersonDetail>(personalDetailsUrl.GetHashCode().ToString());

            // Assert
            Assert.Null(cachedDataBefore); // should be null before fetching data
            Assert.NotNull(result); // should not be null as customer data exists
            Assert.NotNull(cachedDataAfter); // should not be null after fetching data
            Assert.Equal(result.FirstName, cachedDataAfter.FirstName); // should be same as fetched data
            Assert.Equal(result.LastName, cachedDataAfter.LastName); // should be same as fetched data
        }
    }
}