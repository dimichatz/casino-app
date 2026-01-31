using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data
{
    public class AppSetting : ModifiableEntity
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}