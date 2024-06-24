using Logic.Dtos;

namespace Logic.Interfaces
{
    public interface IFintachartsApiService
    {
        Task<ProvidersResponse> GetProviders();

        Task<AssetsResponse> GetAssets(string provider, string kind);

        Task<Dictionary<string, List<string>>> GetAllAssetsInstrumentalKeys();
    }
}
