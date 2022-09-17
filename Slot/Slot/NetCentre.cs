using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
namespace Slot
{
    internal static class NetCentre
    {
        internal static readonly HttpClient _client;
        static NetCentre()
        {
            _client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = true, AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate }) { MaxResponseContentBufferSize = int.MaxValue, Timeout = TimeSpan.FromSeconds(60) };
        }
        public static async Task<string> ReadAsJsonAsync(string uri)
        {
            try
            {
                return (await _client.GetStringAsync(uri)).Trim();
            }
            catch
            {
                _client.CancelPendingRequests();
                return "{}";
            }
        }
        public static async Task<bool> PostAsJsonAsync(string uri,string json)
        {
            try
            {
                return (await _client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"))).IsSuccessStatusCode;
            }
            catch
            {
                _client.CancelPendingRequests();
                return false;
            }
        }
    }
}
