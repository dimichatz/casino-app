namespace CasinoAppBackend.Exceptions
{
    public class ExternalServiceInvalidResponseException : AppException
    {
        private static readonly string DEFAULT_CODE = "InvalidResponse";

        public ExternalServiceInvalidResponseException(string code, string message) 
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}
