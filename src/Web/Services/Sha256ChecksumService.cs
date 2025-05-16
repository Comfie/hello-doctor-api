using System.Security.Cryptography;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Web.Services;

public class Sha256ChecksumService : IChecksumService
{
    public Task<string> ChecksumBytesToHexAsync(byte[] checksum, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            checksum
                .Select(b => b.ToString("x2"))
                .Aggregate(string.Concat)
        );
    }

    public Task<byte[]> HexToChecksumBytesAsync(string checksum, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Enumerable.Range(0, checksum.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(
                    checksum.Substring(x, 2), 16))
                .ToArray()
        );
    }

    /// <summary>
    ///     Check if the given stream's bytes produce the same checksum as: the provided checksum bytes
    /// </summary>
    /// <param name="providedChecksum"></param>
    /// <param name="dataToChecksum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ChecksumEqualsAsync(byte[] providedChecksum, Stream dataToChecksum,
        CancellationToken cancellationToken = default)
    {
        if (dataToChecksum.CanSeek) dataToChecksum.Position = 0;

        var checksum = await ChecksumAsync(dataToChecksum, cancellationToken);

        return providedChecksum.SequenceEqual(checksum);
    }

    /// <summary>
    ///     Check if the given stream's bytes produce the same checksum as: the provided checksum in string format.
    /// </summary>
    /// <param name="providedChecksumHex">Hexadecimal String representation of a md5 hash</param>
    /// <param name="dataToChecksum">A stream of bytes which should be hashed and compared to the provided checksum</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ChecksumEqualsAsync(string providedChecksumHex, Stream dataToChecksum,
        CancellationToken cancellationToken = default)
    {
        if (dataToChecksum.CanSeek) dataToChecksum.Position = 0;

        var checksum = await ChecksumAsHexAsync(dataToChecksum, cancellationToken);
        return string.Equals(checksum, providedChecksumHex, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    ///     Read a given stream, and hash the resultLocal using MD5, return the hash's byte data directly.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<byte[]> ChecksumAsync(Stream data, CancellationToken cancellationToken = default)
    {
        if (data.CanSeek) data.Position = 0;

        using var algorithm = SHA256.Create();
        var checksum = await algorithm.ComputeHashAsync(data, cancellationToken);

        return checksum;
    }

    /// <summary>
    ///     For a given stream, perform a SHA256 hash of its contents and return the data formatted as a hexadecimal string
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> ChecksumAsHexAsync(Stream data, CancellationToken cancellationToken = default)
    {
        var checksum = await ChecksumAsync(data, cancellationToken);
        return await ChecksumBytesToHexAsync(checksum, cancellationToken);
    }
}