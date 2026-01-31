namespace CasinoAppBackend.Services.Access
{
    public enum PlayerAccessFailure
    {
        None,
        PlayerNotFound,
        AccountInactive,
        KycPending,
        SelfExcluded
    }
}