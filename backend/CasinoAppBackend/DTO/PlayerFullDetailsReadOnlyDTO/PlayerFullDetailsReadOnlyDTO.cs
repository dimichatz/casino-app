
namespace CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO
{
    public class PlayerFullDetailsReadOnlyDTO
    {
        public Guid Id { get; set; }
        public PlayerUserDetailsReadOnlyDTO? UserDetails { get; set; }
        public PlayerAddressDetailsReadOnlyDTO? AddressDetails { get; set; }
        public PlayerKycDetailsReadOnlyDTO? KycDetails { get; set; }
        public PlayerSelfExclusionAndLimitDetailsReadOnlyDTO? SelfExclusionAndLimitDetails { get; set; }
    }
}
