namespace HighLowGameApi.Exceptions
{
    public class SessionNotActiveException : Exception
    {
        public SessionNotActiveException(string? message) 
            : base(message)
        {
        }
    }
}
