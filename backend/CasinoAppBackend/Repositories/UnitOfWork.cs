using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CasinoAppBackendDbContext _context;

        public UnitOfWork(CasinoAppBackendDbContext context)
        {
            _context = context;
        }

        public UserRepository UserRepository => new(_context);
        public AdminRepository AdminRepository => new(_context);
        public PlayerRepository PlayerRepository => new(_context);
        public AccountRepository AccountRepository => new(_context);
        public KycDocumentRepository KycDocumentRepository => new(_context);
        public PlayerLimitRepository PlayerLimitRepository => new(_context);
        public PlayerDetailsAuditRepository PlayerDetailsAuditRepository => new(_context);
        public PlayerBanAuditRepository PlayerBanAuditRepository => new(_context);
        public PlayerSelfExclusionAuditRepository PlayerSelfExclusionAuditRepository => new(_context);
        public PlayerLimitAuditRepository PlayerLimitAuditRepository => new(_context);
        public TransactionRepository TransactionRepository => new(_context);
        public AppSettingRepository AppSettingRepository => new(_context);
        public AttachmentRepository AttachmentRepository => new(_context);
        public GameRepository GameRepository => new(_context);
        public GameSessionRepository GameSessionRepository => new(_context);

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
