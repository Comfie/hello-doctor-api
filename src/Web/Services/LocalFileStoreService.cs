using System.Buffers.Text;
using HelloDoctorApi.Application.Common.Interfaces;
using MemoryStream = System.IO.MemoryStream;

namespace HelloDoctorApi.Web.Services;

public class LocalFileStoreService : IFileStoreService
{
    public string ProviderName => "Local";

    public async Task SaveFile(Stream fileStream, string path, CancellationToken cancellationToken = default)
    {
        await using var file = File.Create(path);
        await fileStream.CopyToAsync(file, cancellationToken);
    }

    public Task<Stream> ReadFile(string path, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Stream>(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    }

    public async Task<byte[]> GetFile(string path, CancellationToken cancellationToken = default)
    {
        await using var fileStream = await ReadFile(path, cancellationToken);
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Generates a {uuid}.{extension} path given a filename
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> GeneratePath(Guid id, string fileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "..", "LocalFileStore");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, $"{id}{extension}");
        return Task.FromResult(path);
    }

    public async Task<string> ReadAsBase64(string path, CancellationToken cancellationToken = default)
    {
        await using var fileStream = await ReadFile(path, cancellationToken);
        using var reader = new MemoryStream();
        await fileStream.CopyToAsync(reader, cancellationToken);
        fileStream.Close();
        return Convert.ToBase64String(reader.ToArray());
    }

    public async Task SaveFileFromBase64(string base64, string path, CancellationToken cancellationToken = default)
    {
        if (!Base64.IsValid(base64)) throw new ArgumentException("Invalid base64 string", nameof(base64));

        using var reader = new MemoryStream(Convert.FromBase64String(base64));
        await SaveFile(reader, path, cancellationToken);
    }

    public async Task<string> ReadJsonAsString(string path, CancellationToken cancellationToken = default)
    {
        await using var fileStream = await ReadFile(path, cancellationToken);
        using var reader = new StreamReader(fileStream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}