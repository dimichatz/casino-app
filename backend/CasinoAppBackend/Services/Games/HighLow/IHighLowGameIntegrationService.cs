using CasinoAppBackend.DTO.Games.HighLow;

namespace CasinoAppBackend.Services.Games.HighLow
{
    public interface IHighLowGameIntegrationService
    {
        Task<StartGameResponseDTO> StartGameAsync(Guid userId);
        Task<StartRoundResultResponseDTO> StartRoundAsync(StartRoundRequestDTO request);
        Task<EndSessionResponseDTO> EndSessionAsync(Guid userId, Guid sessionId);
        Task<EndSessionResponseDTO> TimeoutSessionAsync(Guid userId, Guid sessionId);
    }
}
