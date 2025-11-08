namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IFileStoreService
{
    public Task SaveFile(Stream fileStream, string path, CancellationToken cancellationToken = default);
    public Task<Stream> ReadFile(string path, CancellationToken cancellationToken = default);
    public Task<byte[]> GetFile(string path, CancellationToken cancellationToken = default);
    public Task<string> GeneratePath(Guid id, string fileName, CancellationToken cancellationToken = default);
    public Task<string> ReadAsBase64(string path, CancellationToken cancellationToken = default);
    public Task<string> ReadJsonAsString(string path, CancellationToken cancellationToken = default);
    public Task SaveFileFromBase64(string base64, string path, CancellationToken cancellationToken = default);
}