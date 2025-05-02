using KycApi.Model;
using KycApi.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycApi.Service.Implementation
{
    public class MockAggregatedKycService

    {

        public static void GenerateMockData()
        {

            //person1
            var ssn1 = "123-45-6789";
            var person1 = new PersonDetail
            {
                FirstName = "John",
                LastName = "Doe"
            };
            var address1 = new Address
            {
                Street = "123 Main St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "USA"
            };
            var address2 = new Address
            {
                Street = "456 Elm St2",
                City = "Springfield",
                State = "IL",
                PostalCode = "62702",
                Country = "USA"
            };
            var email1 = new Email
            {
                EmailAddress = "John.Doe@xyz.com",
                Preferred = true
            };
            var email2 = new Email
            {
                EmailAddress = "John.Doe1@abc.com",
                Preferred = false
            };
            var phone1 = new PhoneNumber
            {
                Number = "123-456-7890",
                Preferred = true
            };
            var phone2 = new PhoneNumber
            {
                Number = "098-765-4321",
                Preferred = false
            };
            var contactDetail1 = new ContactDetail
            {
                Address = new List<Address> { address1, address2 },
                Emails = new List<Email> { email1, email2 },
                PhoneNumbers = new List<PhoneNumber> { phone1, phone2 }
            };



        }
    }
}