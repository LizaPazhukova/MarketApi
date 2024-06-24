using Logic.Dtos;
using Logic.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Logic.Services
{
    public class FintachartsApiService(TokenProvider tokenProvider) : IFintachartsApiService
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string instrumentsUrl = "https://platform.fintacharts.com/api/instruments/v1/instruments";
        private static readonly string providersUrl = "https://platform.fintacharts.com/api/instruments/v1/providers";

        public async Task<ProvidersResponse> GetProviders()
        {
            return await GetAsync<ProvidersResponse>(providersUrl);
        }

        public async Task<AssetsResponse> GetAssets(string provider, string kind)
        {
            return await GetAsync<AssetsResponse>($"{instrumentsUrl}?provider={provider}&king={kind}");
        }

        public async Task<Dictionary<string, List<string>>> GetAllAssetsInstrumentalKeys()
        {
            var instrumentalKeysDictionary = new Dictionary<string, List<string>>();
            var providers = await GetProviders();
            foreach(var provider in providers.Data)
            {
                var instrumentalKeys = new List<string>();
                int currentPage = 1;
                int totalPages;
                do
                {
                    var response = await GetAsync<AssetsResponse>($"{instrumentsUrl}?provider={provider}&kind=forex&page={currentPage}");;

                    if (response != null)
                    {
                        instrumentalKeys.AddRange(response.Data.Select(x => x.Id));
                        totalPages = response.Paging.Pages;
                        currentPage++;
                    }
                    else
                    {
                        break; // Exit loop if response is null
                    }

                } while (currentPage <= totalPages);
                instrumentalKeysDictionary.Add(provider, instrumentalKeys);
            }
            return instrumentalKeysDictionary;
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, url)
            {
                Content = content
            };

            var token = await tokenProvider.GetAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return request;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content = null)
        {
            var request = await CreateRequestAsync(method, url, content);

            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var token = await tokenProvider.GetAccessTokenAsync();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request = await CreateRequestAsync(method, url, content);
                response = await client.SendAsync(request);
            }

            return response;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await SendAsync(HttpMethod.Get, url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
