namespace CasinoAppBackend.Data
{
    public class Attachment : ModifiableEntity
    {
        public string FileName { get; set; } = null!;
        public string BlobName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public Guid KycDocumentId { get; set; }

        public virtual KycDocument KycDocument { get; set; } = null!;
    }
}
