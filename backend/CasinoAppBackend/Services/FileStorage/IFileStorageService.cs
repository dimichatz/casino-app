namespace CasinoAppBackend.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteAsync(string fileName);
        string GetSasUrl(string blobName, int minutesValid = 10);
    }
}
