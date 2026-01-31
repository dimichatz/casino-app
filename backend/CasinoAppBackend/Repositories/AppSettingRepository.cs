using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class AppSettingRepository : BaseRepository<AppSetting>, IAppSettingRepository
    {
        public AppSettingRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<AppSetting?> GetByKeyAsync(string key) =>
            await context.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
    }
}
