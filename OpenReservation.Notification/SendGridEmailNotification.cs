using SendGrid;
using WeihanLi.Common;

namespace OpenReservation.Notification;

public sealed class SendGridEmailNotification
    (IConfiguration configuration, ISendGridClient client, ILogger<SendGridEmailNotification> logger) 
    : INotification
{
    private readonly ILogger<SendGridEmailNotification> _logger = logger;
    private readonly SendGridEmailConfiguration _emailConfiguration = Guard.NotNull(configuration.GetRequiredSection("SendGrid")
        .Get<SendGridEmailConfiguration>());
    
    public async Task SendAsync(NotificationRequest request)
    {
        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
            new SendGrid.Helpers.Mail.EmailAddress(_emailConfiguration.SourceEmail, _emailConfiguration.SourceName),
            new SendGrid.Helpers.Mail.EmailAddress(request.To),
            request.Subject ?? "Email Notification",
            null,
            request.Text
        );

        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode is System.Net.HttpStatusCode.OK 
            or System.Net.HttpStatusCode.Created 
            or System.Net.HttpStatusCode.Accepted
            )
        {
            _logger.LogInformation("Email Sent for {To}, message: {Message} successfully sent",
                request.To, request.Text);
        }
        else
        {
            var errorMessage = await response.Body.ReadAsStringAsync();
            _logger.LogError("Send email response with code {StatusCode} and body {ErrorMessage}, subject: {Subject}",
                response.StatusCode, errorMessage, request.Subject);
        }
    }
}

public sealed class SendGridEmailConfiguration
{
    public string SourceEmail { get; set; } = "noreply@weihanli.xyz";
    public string SourceName { get; set; } = "Notification Service";
}