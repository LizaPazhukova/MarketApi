﻿using Newtonsoft.Json;

namespace Logic.Dtos;

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }
}