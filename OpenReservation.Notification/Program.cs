using OpenReservation.Notification;
using SendGrid;
using WeihanLi.Web.Authentication;
using WeihanLi.Web.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<DingBotNotification>();
builder.Services.AddKeyedSingleton<INotification, DingBotNotification>(NotificationType.DingBot);
builder.Services.AddHttpClient<WeChatCorpAppNotification>();
builder.Services.AddKeyedSingleton<INotification, WeChatCorpAppNotification>(NotificationType.WeChatCorpApp);
builder.Services.AddSingleton<ISendGridClient>(new SendGridClient(builder.Configuration.GetRequiredAppSetting("SendGridApiKey")));
builder.Services.AddKeyedSingleton<INotification, SendGridEmailNotification>(NotificationType.Email);

builder.Services.AddAuthentication()
    .AddApiKey(options => options.ApiKey = builder.Configuration.GetRequiredAppSetting("AuthApiKey"));
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication()
    .UseAuthorization();

var healthChecks = app.MapGroup("/api/health");
healthChecks.MapGet("/live", () => Results.Ok()).ShortCircuit();
healthChecks.MapGet("/ready", () => Results.Ok()).ShortCircuit();

app.MapRuntimeInfo().ShortCircuit();
app.MapPost("/api/notification/{notificationType}", async (NotificationType notificationType, NotificationRequest request, HttpContext context) =>
{
    var notification = context.RequestServices.GetRequiredKeyedService<INotification>(notificationType);
    await notification.SendAsync(request);
    return Results.Ok();
}).WithOpenApi().RequireAuthorization();

app.Run();
