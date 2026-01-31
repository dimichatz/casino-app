namespace CasinoAppBackend.Services.Access
{
    public interface IPlayerAccessValidator
    {
        Task<PlayerAccessResult> ValidateAsync(Guid userId);
    }
}