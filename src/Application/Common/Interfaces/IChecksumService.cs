namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IChecksumService
{
    Task<string> ChecksumBytesToHexAsync(byte[] checksum, CancellationToken cancellationToken = default);
    Task<byte[]> HexToChecksumBytesAsync(string checksum, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the given stream's bytes produce the same checksum as: the provided checksum bytes
    /// </summary>
    /// <param name="providedChecksum"></param>
    /// <param name="dataToChecksum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ChecksumEqualsAsync(byte[] providedChecksum, Stream dataToChecksum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the given stream's bytes produce the same checksum as: the provided checksum in string format.
    /// </summary>
    /// <param name="providedChecksumHex">Hexadecimal string representation of a hash</param>
    /// <param name="dataToChecksum">A stream of bytes which should be hashed and compared to the provided checksum</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ChecksumEqualsAsync(string providedChecksumHex, Stream dataToChecksum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a given stream, and hash the resultLocal using the underlying algorithm, return the hash's byte data directly.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<byte[]> ChecksumAsync(Stream data, CancellationToken cancellationToken = default);

    /// <summary>
    /// For a given stream, perform a hash of its contents and return the data formatted as a hexadecimal string
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> ChecksumAsHexAsync(Stream data, CancellationToken cancellationToken = default);
}