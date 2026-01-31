using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IAppSettingRepository
    {
        Task<AppSetting?> GetByKeyAsync(string key);
    }
}
