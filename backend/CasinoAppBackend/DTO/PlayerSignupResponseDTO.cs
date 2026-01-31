namespace CasinoAppBackend.DTO
{
    public class PlayerSignupResponseDTO
    {
        public string AccessToken { get; set; } = null!;
        public PlayerReadOnlyDTO User { get; set; } = null!;
    }
}
