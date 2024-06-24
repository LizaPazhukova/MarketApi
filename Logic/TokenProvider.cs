using Logic.Dtos;
using Logic.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Logic
{
    public class TokenProvider(IOptions<ClientSettings> clientSettings)
    {
        private string _accessToken;
        private string _refreshToken;
        private DateTime _accessTokenExpiration;
        private DateTime _refreshTokenExpiration;
        private readonly HttpClient _httpClient = new HttpClient();
        private ClientSettings _clientSettings = clientSettings.Value;
        private readonly string AuthorizeUrl = "https://platform.fintacharts.com/identity/realms/fintatech/protocol/openid-connect/token";


        public async Task<string> GetAccessTokenAsync()
        {
            _clientSettings = clientSettings.Value;
            if (string.IsNullOrEmpty(_accessToken) || _accessTokenExpiration < DateTime.UtcNow)
            {
                await RefreshTokensAsync();
            }
            return _accessToken;
        }

        private async Task RefreshTokensAsync()
        {
            if (string.IsNullOrEmpty(_refreshToken) || _refreshTokenExpiration < DateTime.UtcNow)
            {
                // Obtain new refresh token and access token logic
                await ObtainNewRefreshTokenAsync();
            }
            else
            {
                // Refresh the access token using the refresh token
                await RefreshAccessTokenAsync();
            }
        }

        private async Task ObtainNewRefreshTokenAsync()
        {
            var response = await _httpClient.PostAsync(AuthorizeUrl, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "app-cli" },
                { "username", _clientSettings.UserName },
                { "password", _clientSettings.Password }
            }));

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            _accessToken = tokenResponse.AccessToken;
            _refreshToken = tokenResponse.RefreshToken;
            _accessTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            _refreshTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.RefreshExpiresIn); 
        }

        private async Task RefreshAccessTokenAsync()
        {
            // Implement your logic to refresh the access token using the refresh token
            var response = await _httpClient.PostAsync(AuthorizeUrl, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", _refreshToken }
            }));

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            _accessToken = tokenResponse.AccessToken;
            _accessTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        }
    }
}
