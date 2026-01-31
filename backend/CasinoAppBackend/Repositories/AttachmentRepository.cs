using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class AttachmentRepository : BaseRepository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public void Remove(Attachment attachment)
        {
            dbSet.Remove(attachment);
        }

        public async Task<Attachment?> GetByKycDocumentIdAsync(Guid kycDocumentId) =>
            await context.Attachments.FirstOrDefaultAsync(a => a.KycDocumentId == kycDocumentId);
    }
}
