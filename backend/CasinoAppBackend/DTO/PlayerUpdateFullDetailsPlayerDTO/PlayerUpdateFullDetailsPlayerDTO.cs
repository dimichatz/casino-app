namespace CasinoAppBackend.DTO.PlayerUpdateFullDetailsPlayerDTO
{
    public class PlayerUpdateFullDetailsPlayerDTO
    {
        public PlayerUpdateUserDetailsPlayerDTO? UserDetails { get; set; }
        public PlayerUpdateAddressDetailsPlayerDTO? AddressDetails { get; set; }
        public PlayerUpdateSelfExclusionDetailsPlayerDTO? SelfExclusionDetails { get; set; }
        public PlayerUpdateLimitDetailsPlayerDTO? LimitDetails { get; set; }
    }
}