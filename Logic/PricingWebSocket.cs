using Logic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Logic;

public class TokenAwareWebSocket(Uri webSocketUri, TokenProvider tokenProvider, IFintachartsApiService fintachartsApiService, IServiceProvider _serviceProvider)
{
    private static ClientWebSocket _webSocket = new ClientWebSocket();
    private CancellationTokenSource _cts = new CancellationTokenSource();

    public async Task ConnectAsync()
    {
        await EnsureTokenAsync();
        await _webSocket.ConnectAsync(webSocketUri, _cts.Token);
    }

    private async Task EnsureTokenAsync()
    {
        var token = await tokenProvider.GetAccessTokenAsync();
        _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
    }

    public async Task SendSubscribeMarketDataAsync()
    {
        var instrumentalKeysDictionary = await fintachartsApiService.GetAllAssetsInstrumentalKeys();
        foreach (KeyValuePair<string, List<string>> kvp in instrumentalKeysDictionary)
        {
            foreach (var instrId in kvp.Value)
            {
                var subscribeMessage = new 
                {
                    type = "l1-subscription",
                    id = "1",
                    instrumentId = instrId,
                    provider = kvp.Key,
                    subscribe = true,
                    kinds = new List<string> { "ask", "bid", "last" }
                };
                var message = JsonConvert.SerializeObject(subscribeMessage);
                await SendAsync(message);
            }
        }
    }

    public async Task SendAsync(string message)
    {
        var encodedMessage = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);
        try { await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _cts.Token); }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task ReceiveAsync()
    {
        var buffer = new byte[8096]; 
        while (_webSocket.State == WebSocketState.Open)
        {
            var messageBuilder = new StringBuilder();
            WebSocketReceiveResult result;
            do
            {
                result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                var messagePart = Encoding.UTF8.GetString(buffer, 0, result.Count);
                messageBuilder.Append(messagePart);
            } while (!result.EndOfMessage);

            var fullMessage = messageBuilder.ToString();

            using (var scope = _serviceProvider.CreateScope())
            {
                var marketDataService = scope.ServiceProvider.GetRequiredService<IMarketDataService>();
                await marketDataService.HandleMarketData(fullMessage);
            }
        }
    }

    private async Task HandleReconnectAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", _cts.Token);
        _webSocket.Dispose();
        _webSocket = new ClientWebSocket();

        await ConnectAsync();
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", _cts.Token);
        _webSocket.Dispose();
        _cts.Cancel();
    }
}
