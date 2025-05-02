using KycApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycApi.Service.Interface
{
    public interface IAggregatedKycService
    {
        public Task<AggregatedKyc?> GetAggregatedKycData(string ssn);

        public Task<PersonDetail?> GetPersonalData(string ssn);

        public Task<ContactDetail?> GetContactDetails(string ssn);

        public Task<KycForm?> GetKycFormData(string ssn, string date);
    }
}