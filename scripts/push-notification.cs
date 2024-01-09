using System.Net.Http.Json;

Console.WriteLine(Environment.GetEnvironmentVariables().ToJson());

var messageTemplate = """
The service {{$env serviceName}} deploy with version {{$env imageName}}
[Amazing]
""";
var message = await WeihanLi.Common.Template.TemplateEngine.CreateDefault()
    .RenderAsync(messageTemplate);
Console.WriteLine(message);

var webhookUrl = Environment.GetEnvironmentVariable("DingBotWebhookUrl");
if (string.IsNullOrEmpty(webhookUrl))
{
    Console.WriteLine("WebhookUrl not found");
    return;
}

using var response = await HttpHelper.HttpClient.PostAsJsonAsync(webhookUrl, new
    {
        msgtype = "text",
        text = new
        {
            content = message
        }
    });
response.EnsureSuccessStatusCode();
