namespace OpenReservation.Notification;

public sealed class DingBotNotification : INotification
{
    private readonly HttpClient _httpClient;
    private readonly string _webhookUrl;

    public DingBotNotification(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _webhookUrl = configuration.GetRequiredAppSetting("DingBotWebhookUrl");
    }
    
    public async Task SendAsync(NotificationRequest request)
    {
        using var response = await _httpClient.PostAsJsonAsync(_webhookUrl, new
        {
            msgtype = "text",
            text = new
            {
                content = request.GetMessage()
            }
        });
        response.EnsureSuccessStatusCode();
    }
}