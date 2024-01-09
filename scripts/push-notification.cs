using System.Net.Http.Json;

var webhookUrl = Environment.GetEnvironmentVariable("DingBotWebhookUrl");
if (string.IsNullOrEmpty(webhookUrl)) return;

var messageTemplate = """
The service {{env serviceName}} deploy with version {{env imageName}}
Repo: {{env Build.Repository.Uri}}
[Amazing]
""";
var message = await WeihanLi.Common.Template.TemplateEngine.CreateDefault()
    .RenderAsync(messageTemplate);
Console.WriteLine(message);
using var response = await HttpHelper.HttpClient.PostAsJsonAsync(webhookUrl, new
    {
        msgtype = "text",
        text = new
        {
            content = message
        }
    });
response.EnsureSuccessStatusCode();
