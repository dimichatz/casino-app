namespace CasinoAppBackend.DTO
{
    public class AttachmentReadOnlyDTO
    {
        public string? FileName { get; set; }
        public string? DownloadUrl { get; set; }   // SAS URL
        public string? ContentType { get; set; }
        public string? Extension { get; set; }
    }
}
