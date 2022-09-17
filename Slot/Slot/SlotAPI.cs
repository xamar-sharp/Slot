using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using Slot.ViewModels;
using System.Collections.ObjectModel;
namespace Slot
{
    public sealed class Data
    {
        public string FileURI { get; set; }
        public long Length { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Command DeleteCommand { get; set; }
        public Data()
        {
            DeleteCommand = new Command(async (obj) =>
            {
                if (!await SlotAPI.DeleteFile(FileURI))
                {
                    await Shell.Current.DisplayAlert(Resource.Message, Resource.APIError, Resource.Accept);
                }
                (obj as ObservableCollection<Data>).Remove(this);
            });
        }
    }
    public sealed class FileModel
    {
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
    public sealed class UserInfo
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public byte[] Icon { get; set; }
    }
    public sealed class ResponseModel
    {
        public string Login { get; set; }
        public byte[] Icon { get; set; }
        public string Jwt { get; set; }
        public string Refresh { get; set; }
        public DateTime UtcJwtExpiration { get; set; }
    }
    public static class SlotAPI
    {
        private static readonly HttpClient _client;
        static SlotAPI()
        {
            _client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, AllowAutoRedirect = true })
            {
                Timeout = TimeSpan.FromMinutes(3),
                BaseAddress = new Uri($"http://{Prefixes.IP}:5000/")
            };
        }
        public static async ValueTask<bool> Authorize(UserInfo info = null)
        {
            try
            {
                if (info != null)
                {
                    var message = await _client.PostAsync("authorization", new StringContent(JsonConvert.SerializeObject(info), Encoding.UTF8, "application/json"));
                    message.EnsureSuccessStatusCode();
                    App.SaveAuthInfo(JsonConvert.DeserializeObject<ResponseModel>(await message.Content.ReadAsStringAsync()));
                }
                if (App.LocalAuth?.UtcJwtExpiration <= DateTime.UtcNow)
                {
                    var message = await _client.GetAsync($"authorization/{App.LocalAuth.Refresh}");
                    message.EnsureSuccessStatusCode();
                    App.SaveAuthInfo(JsonConvert.DeserializeObject<ResponseModel>(await message.Content.ReadAsStringAsync()));
                }
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.LocalAuth.Jwt);
                return true;
            }
            catch(Exception ex)
            {
                _client.CancelPendingRequests();
                return false;
            }
        }
        public static async ValueTask<bool> PostFile(FileModel model)
        {
            try
            {
                if(await Authorize())
                {
                    (await _client.PostAsync("memen", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"))).EnsureSuccessStatusCode();
                    return true;
                }
                return App.CheckAuth();
            }
            catch(Exception ex)
            {
                _client.CancelPendingRequests();
                return false;
            }
        }
        public static async ValueTask<IList<Data>> GetDatas()
        {
            try
            {
                if (await Authorize())
                {
                    var mes = await _client.GetAsync("memen",HttpCompletionOption.ResponseContentRead);
                    return JsonConvert.DeserializeObject<Data[]>(await mes.Content.ReadAsStringAsync()).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                _client.CancelPendingRequests();
                return null;
            }
        }
        public static async ValueTask<bool> DeleteFile(string path)
        {
            try
            {
                if (await Authorize())
                {
                    (await _client.DeleteAsync($"memen/{path}")).EnsureSuccessStatusCode();
                    return true;
                }
                return App.CheckAuth();
            }
            catch (Exception ex)
            {
                _client.CancelPendingRequests();
                return false;
            }
        }
    }
}
