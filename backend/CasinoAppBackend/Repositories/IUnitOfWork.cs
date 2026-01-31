using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        AdminRepository AdminRepository { get; }
        PlayerRepository PlayerRepository { get; }
        AccountRepository AccountRepository { get; }
        KycDocumentRepository KycDocumentRepository { get; }
        PlayerLimitRepository PlayerLimitRepository { get; }
        PlayerDetailsAuditRepository PlayerDetailsAuditRepository { get; }
        PlayerBanAuditRepository PlayerBanAuditRepository { get; }
        PlayerSelfExclusionAuditRepository PlayerSelfExclusionAuditRepository { get; }
        PlayerLimitAuditRepository PlayerLimitAuditRepository { get; }
        TransactionRepository TransactionRepository { get; }
        AppSettingRepository AppSettingRepository { get; }
        AttachmentRepository AttachmentRepository { get; }
        GameRepository GameRepository { get; }
        GameSessionRepository GameSessionRepository { get; }

        Task<bool> SaveAsync();
    }
}
