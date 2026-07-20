namespace Assignment1_EventSignup.Services
{
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads a file to Azure Blob Storage and returns its public URL.
        /// </summary>
        Task<string> UploadFileAsync(IFormFile file);
    }
}
