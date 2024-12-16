using System.Net;
using System.Text;
using System.Text.Json;

namespace Dogs
{
    internal class DogsBot
    {
        public readonly DogsQuery PubQuery;
        private readonly ProxyType[] PubProxy;
        public readonly DogsJoinResponse UserDetail;
        public readonly bool HasError;
        public readonly string ErrorMessage;
        public readonly string IPAddress;

        public DogsBot(DogsQuery Query, ProxyType[] Proxy)
        {
            PubQuery = Query;
            PubProxy = Proxy;
            IPAddress = GetIP().Result;
            PubQuery.Auth = GetSession();
            if (!string.IsNullOrEmpty(PubQuery.Auth))
            {
                var Login = DogsLogin().Result;
                if (Login is not null)
                {
                    UserDetail = Login;
                    HasError = false;
                    ErrorMessage = "";
                }
                else
                {
                    UserDetail = new();
                    HasError = true;
                    ErrorMessage = "login failed";
                }
            }
            else
            {
                UserDetail = new();
                HasError = true;
                ErrorMessage = "login failed";
            }
        }

        private async Task<string> GetIP()
        {
            HttpClient client;
            var FProxy = PubProxy.Where(x => x.Index == PubQuery.Index);
            if (FProxy.Any())
            {
                if (!string.IsNullOrEmpty(FProxy.ElementAtOrDefault(0)?.Proxy))
                {
                    var handler = new HttpClientHandler() { Proxy = new WebProxy() { Address = new Uri(FProxy.ElementAtOrDefault(0)?.Proxy ?? string.Empty) } };
                    client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 30) };
                }
                else
                    client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            }
            else
                client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync($"https://httpbin.org/ip");
            }
            catch (Exception ex)
            {
            }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<Httpbin>(responseStream);
                    return responseJson.Origin;
                }
            }

            return string.Empty;
        }

        private string GetSession()
        {
            var vw = new TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "dogshouse_bot", "https://onetime.dog/");
            string url = vw.Get_URL().Result;

            if (url != string.Empty)
                return url.Split(new string[] { "tgWebAppData=" }, StringSplitOptions.None)[1].Split(new string[] { "&tgWebAppVersion" }, StringSplitOptions.None)[0];

            return string.Empty;
        }

        private async Task<DogsJoinResponse?> DogsLogin()
        {
            var DAPI = new DogsApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var serializedRequestContent = new StringContent(PubQuery.Auth, Encoding.UTF8);
            var httpResponse = await DAPI.DAPIPost("https://api.onetime.dog/join", serializedRequestContent);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<DogsJoinResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<DogsRewardsResponse?> DogsRewards()
        {
            var DAPI = new DogsApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await DAPI.DAPIGet($"https://api.onetime.dog/rewards?user_id={UserDetail.TelegramId}");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<DogsRewardsResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<List<DogsCalendarItem>?> DogsCalendar()
        {
            var DAPI = new DogsApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await DAPI.DAPIGet($"https://api.onetime.dog/advent/calendar?user_id={UserDetail.TelegramId}");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<List<DogsCalendarItem>>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> DogsCalendarCheck(int day)
        {
            var DAPI = new DogsApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await DAPI.DAPIPost($"https://api.onetime.dog/advent/calendar/check?user_id={UserDetail.TelegramId}&day={day}", (HttpContent)null);
            if (httpResponse is not null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }
    }
}
