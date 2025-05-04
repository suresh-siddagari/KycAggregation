using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycApi.Service.Interface
{
    public interface IDataService
    {
        public Task<T?> GetData<T>(string url);
    }
}