using CasinoAppBackend.DTO.Games.HighLow;

namespace CasinoAppBackend.Services.Games.HighLow
{
    public interface IHighLowGameService
    {
        Task<StartGameResponseDTO> StartGameAsync(Guid userId);
        Task<StartRoundResultResponseDTO> StartRoundAsync(Guid userId, StartRoundRequestDTO request);
        Task<EndSessionResponseDTO> EndSessionAsync(Guid userId, Guid sessionId);
        Task<EndSessionResponseDTO> TimeoutSessionAsync(Guid userId, Guid sessionId);
        Task<ConfigResponseDTO> GetConfigAsync(Guid userId);

    }
}
