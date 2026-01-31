using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IKycDocumentRepository
    {
        Task<KycDocument?> GetByPlayerIdAsync(Guid playerId);
    }
}