using Logic.Interfaces;
using Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Logic;

public static class LogicModuleDefinition
{
    public static IServiceCollection AddLogic(this IServiceCollection collection)
    {
        collection.AddSingleton<TokenProvider>();
        collection.AddSingleton<IFintachartsApiService, FintachartsApiService>();
        collection.AddScoped<IMarketDataService, MarketDataService>();

        // Register TokenAwareWebSocket
        collection.AddSingleton(provider =>
        {
            var tokenProvider = provider.GetRequiredService<TokenProvider>();
            var fintachartsApiService = provider.GetRequiredService<IFintachartsApiService>();
            var webSocketUri = new Uri($"wss://platform.fintacharts.com/api/streaming/ws/v1/realtime");
            return new TokenAwareWebSocket(webSocketUri, tokenProvider, fintachartsApiService, provider);
        });

        return collection;
    }
}
