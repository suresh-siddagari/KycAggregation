using KycApi.Service.Implementation;
using Xunit;

namespace KycApi.Service.Test
{
    public class SwedishSsnValidatorTests
    {
        [Theory]
        [InlineData("19810101-2386")] // Valid: Luhn for 810101238 is 6
        [InlineData("810101-2386")]   // Valid: Luhn for 810101238 is 6
        [InlineData("198101012386")]  // Valid: Luhn for 810101238 is 6
        [InlineData("8101012386")]    // Valid: Luhn for 810101238 is 6
        [InlineData("19120211+2387")] // Valid: (1912-02-11) Luhn for 120211238 is 7
        [InlineData("120211+2387")]   // Valid: (1912-02-11) Luhn for 120211238 is 7
        [InlineData("19810161-2383")] // Valid: (Coord day 61->01) Luhn for 810161238 is 3
        [InlineData("810161-2383")]   // Valid: (Coord day 61->01) Luhn for 810161238 is 3
        public void IsValidSsn_ValidSsns_ReturnsTrue(string ssn)
        {
            Assert.True(SwedishSsnValidator.IsValidSsn(ssn), $"Expected true for {ssn}");
        }

        [Theory]
        [InlineData("19810101-2388")] // Invalid checksum (correct is 2386)
        [InlineData("810101-2388")]   // Invalid checksum (correct is 2386)
        [InlineData("198101012388")]  // Invalid checksum (correct is 2386)
        [InlineData("8101012388")]    // Invalid checksum (correct is 2386)
        [InlineData("19120211+9802")] // Invalid checksum (Luhn for 120211980 is 4, needed 2. Correct for 19120211 is 2387)
        [InlineData("120211+9801")]   // Invalid checksum (Luhn for 120211980 is 4, digit 1. Correct for 19120211 is 2387)
        [InlineData("19810161-2381")] // Invalid checksum (correct for 810161238 is 3)
        [InlineData("810161-2381")]   // Invalid checksum (correct for 810161238 is 3)
        [InlineData("12345")]         // Invalid length (too short)
        [InlineData("1234567890123")] // Invalid length (too long)
        [InlineData("810101-238A")]   // Invalid character (contains 'A')
        [InlineData("19811301-2380")] // Invalid month (13)
        [InlineData("19810229-2380")] // Invalid day (Feb 29 in non-leap year 1981)
        [InlineData("19810132-2380")] // Invalid day (32)
        [InlineData("19810192-2380")] // Invalid coordination number day (92) - days 61-91 are valid
        [InlineData("21000229-2310")] // Invalid leap day (2100 not a leap year)
        public void IsValidSsn_InvalidSsns_ReturnsFalse(string ssn)
        {
            Assert.False(SwedishSsnValidator.IsValidSsn(ssn), $"Expected false for {ssn}");
        }

        [Theory]
        [InlineData("19700101-0003")] // Valid: (1970-01-01) Luhn for 700101000 is 3
        [InlineData("700101-0003")]   // Valid: (1970-01-01) Luhn for 700101000 is 3
        [InlineData("18700101+0003")] // Valid: (1870-01-01, represented with century for clarity) Luhn for 700101000 is 3. Input as "700101+0003"
        [InlineData("700101+0003")]   // Valid: (1870-01-01) Luhn for 700101000 is 3
        [InlineData("19800229-2319")] // Valid: (Leap 1980-02-29) Luhn for 800229231 is 9
        [InlineData("800229-2319")]   // Valid: (Leap 1980-02-29) Luhn for 800229231 is 9
        [InlineData("20000229-2316")] // Valid: (Leap 2000-02-29) Luhn for 000229231 is 6
        public void IsValidSsn_SpecificValidSsns_ReturnsTrue(string ssn)
        {
            Assert.True(SwedishSsnValidator.IsValidSsn(ssn), $"Expected true for {ssn}");
        }

        [Fact]
        public void IsValidSsn_ValidLeapYearSsns_ReturnsTrue()
        {
            Assert.True(SwedishSsnValidator.IsValidSsn("20000229-2316")); // Born 2000 (leap) - Corrected
            Assert.True(SwedishSsnValidator.IsValidSsn("000229-2316"));   // Born 2000 (leap) - Corrected
            Assert.True(SwedishSsnValidator.IsValidSsn("19960229-2386")); // Born 1996 (leap) - Corrected
            Assert.True(SwedishSsnValidator.IsValidSsn("960229-2386"));   // Born 1996 (leap) - Corrected
        }

        [Fact]
        public void IsValidSsn_InvalidLeapYearSsns_ReturnsFalse()
        {
            Assert.False(SwedishSsnValidator.IsValidSsn("19970229-2312")); // Not a leap year
            Assert.False(SwedishSsnValidator.IsValidSsn("970229-2312"));   // Not a leap year (interpreted as 1997)
            Assert.False(SwedishSsnValidator.IsValidSsn("21000229-2310")); // 2100 not a leap year
        }

        [Fact]
        public void IsValidSsn_SsnWithPlusSign_ReturnsCorrectValidation()
        {
            // Born 1920-01-01. Luhn for 200101xxx. Serial 238. Luhn for 200101238 is 0.
            Assert.True(SwedishSsnValidator.IsValidSsn("200101+2380"), "Test 1: 1920-01-01 (Corrected)"); 
            Assert.False(SwedishSsnValidator.IsValidSsn("200101+2382"), "Test 2: 1920-01-01 (Incorrect Checksum)");

            // Born 1910-01-01. Luhn for 100101xxx. Serial 238. Luhn for 100101238 is 2.
            Assert.True(SwedishSsnValidator.IsValidSsn("100101+2382"), "Test 3: 1910-01-01 (Corrected)"); 
            Assert.False(SwedishSsnValidator.IsValidSsn("100101+2380"), "Test 4: 1910-01-01 (Incorrect Checksum)");

            // Born 1912-02-11. Luhn for 120211xxx. Serial 238. Luhn for 120211238 is 7.
            Assert.True(SwedishSsnValidator.IsValidSsn("19120211+2387"), "Test 5: 1912-02-11 (Corrected)");
            Assert.True(SwedishSsnValidator.IsValidSsn("120211+2387"), "Test 6: 1912-02-11 (10-digit with +, Corrected)");
            Assert.False(SwedishSsnValidator.IsValidSsn("120211+9802"), "Test 7: Original example 120211+9802 from problem (Invalid Luhn)");
        }
    }
}
