namespace CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO
{
    public class PlayerUpdateFullDetailsAdminDTO
    {
        public PlayerUpdateUserStatusDetailsAdminDTO? UserStatusDetails { get; set; }
        public PlayerUpdateUserDetailsAdminDTO? UserDetails { get; set; }
        public PlayerUpdateAddressDetailsAdminDTO? AddressDetails { get; set; }
        public PlayerUpdateKycDetailsAdminDTO? KycDetails { get; set; }
        public string? Comment { get; set; }
    }
}
