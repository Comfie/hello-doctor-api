using Ardalis.Result;

namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IDocumentService
{
    Task<Result<long>> SavePrescriptionFile(Stream file, long mainMemberId,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> GetPrescriptionFile(long id, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeletePrescriptionFile(long id, CancellationToken cancellationToken = default);

    //File store service
}