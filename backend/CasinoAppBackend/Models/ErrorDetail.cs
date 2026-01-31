namespace CasinoAppBackend.Models
{
    public class ErrorDetail
    {
        public string? Type { get; set; }
        public string? Message { get; set; }
        public string? Code { get; set; }

        public ErrorDetail() { }

        public ErrorDetail(string type, string code, string message)
        {
            Code = code;
            Type = type;
            Message = message;
        }
    }
}
