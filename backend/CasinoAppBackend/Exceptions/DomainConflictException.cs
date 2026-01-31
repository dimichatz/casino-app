namespace CasinoAppBackend.Exceptions
{
    public class DomainConflictException : AppException
    {
        public DomainConflictException(string code, string message) 
            : base(code, message)
        {
        }
    }
}
