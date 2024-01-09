using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace OpenReservation.Notification;

public sealed class WeChatCorpAppNotification : INotification
{
    private const string GetTokenUrlFormat
        = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";
    private const string SendMessageUrlFormat =
        "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
    
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly WeCharCorpAppInfo _appInfo;

    public WeChatCorpAppNotification(HttpClient httpClient, IMemoryCache memoryCache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _appInfo = Guard.NotNull(configuration.GetSection("WeChatCorpApp").Get<WeCharCorpAppInfo>());
    }
    
    public async Task SendAsync(NotificationRequest request)
    {
        // get access token
        var accessToken = await _memoryCache.GetOrCreateAsync("wechatCorp_accessToken", async entry =>
        {
            var getTokenUrl = GetTokenUrlFormat.FormatWith(_appInfo.CorpId, _appInfo.AppSecret);
            var accessTokenResponse = await _httpClient.GetFromJsonAsync<AccessTokenResponse>(getTokenUrl);
            if (!string.IsNullOrEmpty(accessTokenResponse?.AccessToken))
            {
                if (accessTokenResponse.ExpiresIn > 0)
                {
                    entry.SlidingExpiration = TimeSpan.FromSeconds(accessTokenResponse.ExpiresIn);
                }
                return accessTokenResponse.AccessToken;
            }
            throw new InvalidOperationException($"Invalid response, {accessTokenResponse?.ErrorCode} {accessTokenResponse?.ErrorMsg}, {accessTokenResponse?.AccessToken}");
        });
        // send message
        var sendMessageUrl = string.Format(SendMessageUrlFormat, accessToken);
        using var response = await _httpClient.PostAsJsonAsync(sendMessageUrl,
            new
            {
                touser = request.To.GetValueOrDefault("@all"),
                msgtype = "text",
                agentid = _appInfo.AppId,
                text = new { content = request.GetMessage() }
            });
        response.EnsureSuccessStatusCode();
    }

    private sealed class AccessTokenResponse
    {
        [JsonPropertyName("errcode")]
        public required int ErrorCode { get; set; }

        [JsonPropertyName("errmsg")]
        public string? ErrorMsg { get; set; }
        
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
    
    private sealed class WeCharCorpAppInfo
    {
        public required string CorpId { get; set; }
        public required string AppId { get; set; }
        public required string AppSecret { get; set; }
    }
}

