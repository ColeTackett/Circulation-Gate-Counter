using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GateCount
{
    public class GateCounter
    {
        private readonly HttpMessageInvoker client;

        public GateCounter()
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(3),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(3),
                MaxConnectionsPerServer = 10
            };

            client = new HttpMessageInvoker(handler);
        }

        public async Task<int> GetGateCount(string url)
        {
            int retryCount = 0;
            const int maxRetryCount = 25;

            while (retryCount < maxRetryCount)
            {
                try
                {
                    HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url), CancellationToken.None);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Parse XML and extract the gate count
                        XDocument xdoc = XDocument.Parse(responseBody);
                        var count1Element = xdoc.Root?.Element("count1");
                        if (count1Element != null)
                        {
                            string gateCountText = count1Element.Value;

                            if (int.TryParse(gateCountText, out int gateCount))
                            {
                                return gateCount;
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions and retry the request
                }

                retryCount++;
            }

            // Return -1 if all attempts failed
            return -1;
        }
    }
}