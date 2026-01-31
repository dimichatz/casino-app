using CasinoAppBackend.Models;

namespace CasinoAppBackend.Exceptions
{
    public class MultipleErrorsException : AppException
    {
        public List<ErrorDetail> Errors { get; }

        public MultipleErrorsException(IEnumerable<ErrorDetail> errors, string code, string message)
            : base (code, message)
        {
            Errors = errors.ToList();
        }
    }
}
