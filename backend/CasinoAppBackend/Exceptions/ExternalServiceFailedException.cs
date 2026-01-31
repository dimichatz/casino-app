namespace CasinoAppBackend.Exceptions
{
    public class ExternalServiceFailedException : AppException
    {
        private static readonly string DEFAULT_CODE = "Failed";

        public ExternalServiceFailedException(string code, string message) 
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}
