using Logic;
using DAL;
using Logic.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.Configure<ClientSettings>(c => builder.Configuration.GetSection("ClientSettings").Bind(c));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDal(builder.Configuration);
builder.Services.AddLogic();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await InitializeWebSocketConnectionAsync(app.Services);

app.Run();

async Task InitializeWebSocketConnectionAsync(IServiceProvider serviceProvider)
{
    var webSocketClient = serviceProvider.GetRequiredService<TokenAwareWebSocket>();

    await webSocketClient.ConnectAsync();

    _ = Task.Run(async () =>
    {
        try
        {
            await webSocketClient.ReceiveAsync();
        }
        catch (Exception ex)
        {
            // Handle exceptions from the background task
            Console.WriteLine($"Error in ReceiveAsync: {ex.Message}");
        }
    });

    await webSocketClient.SendSubscribeMarketDataAsync();
}
