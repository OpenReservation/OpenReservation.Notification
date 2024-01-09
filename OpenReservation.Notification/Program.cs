using OpenReservation.Notification;
using WeihanLi.Web.Authentication;

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

builder.Services.AddAuthentication()
    .AddApiKey(options => options.ApiKey = builder.Configuration.GetRequiredAppSetting("AuthApiKey"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication()
    .UseAuthorization();

app.MapPost("/api/notification/{notificationType}", async (NotificationType notificationType, NotificationRequest request, HttpContext context) =>
{
    var notification = context.RequestServices.GetRequiredKeyedService<INotification>(notificationType);
    await notification.SendAsync(request);
    return Results.Ok();
}).WithOpenApi().RequireAuthorization();

app.Run();
