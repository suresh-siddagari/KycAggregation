using KycApi.Service.Implementation;

namespace KycApi.Service.Test
{
    public class AggregatedKycServiceTest
    {
        // Test method : GetContactDetails
        // Test case: Valid SSN and Customer data does not exist
        // Expected result: Returns null

        [Fact]
        public async Task GetContactDetails_ValidSsn_CustomerDataDoesNotExist_ReturnsNull()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new AggregatedKycService(httpClient);
            string ssn = "19900101-1234";

            // Act
            var result = await service.GetContactDetails(ssn);

            // Assert
            Assert.True(result == null); // should be null as customer data does not exist
        }

        // Test method : GetContactDetails
        // Test case: Valid SSN and Customer data exists
        // Expected result: Contact details of the customer
        [Fact]
        public async Task GetContactDetails_ValidSsn_CustomerDataExists_ReturnsContactDetails()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new AggregatedKycService(httpClient);
            string ssn = "19800115-1234";

            // Act
            var result = await service.GetContactDetails(ssn);

            // Assert
            Assert.NotNull(result); // should not be null as customer data exists
            Assert.True(result.Address.Count > 0); // should have at least one address
            Assert.True(result.Emails.Count > 0); // should have at least one email
            Assert.True(result.PhoneNumbers.Count > 0); // should have at least one phone number
        }
    }
}