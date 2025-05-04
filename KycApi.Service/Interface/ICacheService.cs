using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycApi.Service.Interface
{
    public interface ICacheService
    {
        public T? Get<T>(string key);

        public void Set<T>(string key, T value);
    }
}