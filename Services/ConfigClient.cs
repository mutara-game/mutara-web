using System.Text;
using System.Threading.Tasks;
using Consul;

namespace Mutara.Web.Services {


// TODO: does Consul.ConsulClient do any sort of caching or does it call consul every time?
    public class ConfigClient {
        
        public async Task<string> GetValue(string filename, string key)
        {
            using var client = new ConsulClient();
            // TODO: environment and branch IDs from ... somewhere.
            string fullKey = string.Join("/", "mutara/master/dev", filename, key);
            var getPair = await client.KV.Get(fullKey);
            return getPair == null ? "" : Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
        }
    }
}