using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Serializers
{
    public class JsonNetWsSerializer : IWsSerializer
    {
        public Task<byte[]> SerializeAsync<T>(T value)
        {
            return Task.FromResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        public Task<T> DeserializeAsync<T>(byte[] serialized)
        {
            return this.DeserializeAsync<T>(serialized, 0, serialized.Length);
        }

        public Task<T> DeserializeAsync<T>(byte[] serialized, int offset, int length)
        {
            return Task.FromResult(JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized, offset, length)));
        }
    }
}
