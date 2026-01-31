namespace CasinoAppBackend.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string code, string message)
            : base(code, message)
        {
        }
    }
}
