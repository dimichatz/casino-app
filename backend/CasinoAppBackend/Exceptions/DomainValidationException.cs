namespace CasinoAppBackend.Exceptions
{
    public class DomainValidationException : AppException
    {

        public DomainValidationException(string code, string message)
            : base(code, message)
        {
        }
    }
}
