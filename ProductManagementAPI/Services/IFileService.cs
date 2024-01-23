namespace ProductManagementAPI.Services
{
    public interface IFileService
    {
        Task WriteAllTextAsync(string path, string contents);
    }
}
