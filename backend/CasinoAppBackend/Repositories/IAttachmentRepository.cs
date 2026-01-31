using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IAttachmentRepository
    {
        void Remove(Attachment attachment);
        Task<Attachment?> GetByKycDocumentIdAsync(Guid kycDocumentId);
    }
}
