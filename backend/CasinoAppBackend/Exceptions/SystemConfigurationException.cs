namespace CasinoAppBackend.Exceptions
{
    public class SystemConfigurationException : AppException
    {
        public SystemConfigurationException(string code, string message)
            : base(code, message)
        {
        }
    }
}
