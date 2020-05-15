using System;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;

namespace mutara_web.Services {


// TODO: does Consul.ConsulClient do any sort of caching or does it call consul every time?
    public class ConfigClient {
        private readonly ILogger<ConfigClient> logger;

        public ConfigClient(ILogger<ConfigClient> logger) {
            this.logger = logger;
        }

        public async Task<string> GetValue(string filename, string key) {
            using(var client = new ConsulClient()) {
                // TODO: environment and branch IDs from ... somewhere.
                string fullKey = String.Join("/", "mutara/master/dev", filename, key);
                var getPair = await client.KV.Get(fullKey);
                if (getPair == null) {
                    return "";
                }
                return Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
            }
        }
    }
}