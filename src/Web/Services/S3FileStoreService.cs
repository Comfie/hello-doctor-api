using System.Buffers.Text;
using Amazon.S3;
using Amazon.S3.Model;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using MemoryStream = System.IO.MemoryStream;

namespace HelloDoctorApi.Web.Services;

public class S3FileStoreService : IFileStoreService
{
    private readonly S3FileStoreOptions _options;
    private readonly IAmazonS3 _s3Client;

    public S3FileStoreService(
        IAmazonS3 s3Client,
        IOptions<S3FileStoreOptions> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
    }

    public string ProviderName => "S3";

    public async Task SaveFile(Stream fileStream, string path, CancellationToken cancellationToken = default)
    {
        fileStream.Position = 0;

        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = path,
            InputStream = fileStream,
            AutoCloseStream = false
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
    }

    public async Task<Stream> ReadFile(string path, CancellationToken cancellationToken = default)
    {
        if (path.Contains("LocalFileStore"))
            return new MemoryStream(); // Can't handle existing stream logic

        var listRequest = new ListObjectsV2Request
        {
            BucketName = _options.BucketName,
            Prefix = path,
            MaxKeys = 1
        };

        var listResponse = await _s3Client.ListObjectsV2Async(listRequest, cancellationToken);

        if (listResponse?.S3Objects is null || !listResponse.S3Objects.Any() || listResponse.S3Objects[0].Key != path)
            return new MemoryStream();

        var getRequest = new GetObjectRequest
        {
            BucketName = _options.BucketName,
            Key = path
        };

        var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
        var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<byte[]> GetFile(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = await ReadFile(path, cancellationToken);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Generates a {uuid}.{extension} path given a filename
    /// </summary>
    public Task<string> GeneratePath(Guid id, string fileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var path = $"{id}{extension}";
        return Task.FromResult(path);
    }

    public async Task<string> ReadAsBase64(string path, CancellationToken cancellationToken = default)
    {
        await using var fileStream = await ReadFile(path, cancellationToken);
        using var reader = new MemoryStream();
        await fileStream.CopyToAsync(reader, cancellationToken);
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

    public async Task<string> SaveRegulaResponse(Stream jsonContentStream, Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var fileName = $"RegulaResponse_{documentId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
        var path = await GeneratePath(documentId, fileName, cancellationToken);
        using var memoryStream = new MemoryStream();
        await jsonContentStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        await SaveFile(memoryStream, path, cancellationToken);
        return path;
    }
}

public class S3FileStoreOptions
{
    public const string ConfigurationSection = "S3FileStore";
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}