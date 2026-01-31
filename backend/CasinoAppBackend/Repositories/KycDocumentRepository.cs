using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class KycDocumentRepository : BaseRepository<KycDocument>, IKycDocumentRepository
    {
        public KycDocumentRepository(CasinoAppBackendDbContext context) : base(context)
        {
        }

        public async Task<KycDocument?> GetByPlayerIdAsync(Guid playerId) =>
            await context.KycDocuments.FirstOrDefaultAsync(k => k.PlayerId == playerId);
    }
}
