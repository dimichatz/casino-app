using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data
{
    public class Player : ModifiableEntity
    {
        public Player()
        {
            PlayerLimit = new PlayerLimit();
        }

        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string StreetName { get; set; } = null!;
        public string StreetNumber { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public bool IsAgeVerified { get; set; }
        public bool HasAcceptedTerms { get; set; }
        public bool IsKycVerified { get; set; } = false;
        public bool IsSelfExcluded { get; set; } = false;
        public DateTime? SelfExclusionStart { get; set; }
        public DateTime? SelfExclusionEnd { get; set; }
        public SelfExclusionPeriod? SelfExclusionPeriod { get; set; }
        public Guid UserId { get; set; }
        public string CountryCode { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public virtual Country Country { get; set; } = null!;
        public virtual ICollection<PlayerDetailsAudit> PlayerDetailsAudits { get; set; } = new HashSet<PlayerDetailsAudit>();
        public virtual KycDocument KycDocument { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<PlayerSelfExclusionAudit> PlayerSelfExclusionAudits { get; set; } = new HashSet<PlayerSelfExclusionAudit>();
        public virtual ICollection<PlayerBanAudit> PlayerBanAudits { get; set; } = new HashSet<PlayerBanAudit>();
        public virtual ICollection<PlayerLimitAudit> PlayerLimitAudits { get; set; } = new HashSet<PlayerLimitAudit>();
        public virtual PlayerLimit PlayerLimit { get; set; } = null!;
    }
}
