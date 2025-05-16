using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Commands.UploadPharmacyLogo;

public class UploadPharmacyLogoHandler : IRequestHandler<UploadPharmacyLogoCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UploadPharmacyLogoHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UploadPharmacyLogoCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _context
            .Pharmacies
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (pharmacy is null)
            return Result<bool>.NotFound(new Error("Upload Logo", "Pharmacy not found"));


        string? logoData = null;

        using (var memoryStream = new MemoryStream())
        {
            await request.File.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            logoData = Convert.ToBase64String(fileBytes);
        }

        if (string.IsNullOrEmpty(logoData))
            return Result<bool>.NotFound(new Error("Upload Logo", "Logo not found"));

        pharmacy.Logo = logoData;
        _context.Pharmacies.Update(pharmacy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}