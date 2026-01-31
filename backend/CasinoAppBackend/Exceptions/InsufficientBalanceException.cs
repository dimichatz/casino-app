namespace CasinoAppBackend.Exceptions
{
    public class InsufficientBalanceException : AppException
    {
        private static readonly string DEFAULT_CODE = "InsufficientBalance";

        public InsufficientBalanceException(string code, string message) 
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}
