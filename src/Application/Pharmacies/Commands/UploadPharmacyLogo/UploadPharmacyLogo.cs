using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace HelloDoctorApi.Application.Pharmacies.Commands.UploadPharmacyLogo;

public record UploadPharmacyLogoCommand(long Id, IFormFile File) : IRequest<Result<bool>>;

public class UploadPharmacyLogoCommandValidator : AbstractValidator<UploadPharmacyLogoCommand>
{
    public UploadPharmacyLogoCommandValidator()
    {
    }
}

public class UploadPharmacyLogoCommandHandler : IRequestHandler<UploadPharmacyLogoCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UploadPharmacyLogoCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
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
            return Result.Failure<bool>(new Error("Upload Logo", "Pharmacy not found"));


        string? logoData = null;

        using (var memoryStream = new MemoryStream())
        {
            await request.File.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            logoData = Convert.ToBase64String(fileBytes);
        }

        if (string.IsNullOrEmpty(logoData))
            return Result.Failure<bool>(new Error("Upload Logo", "Logo not found"));

        pharmacy.Logo = logoData;
        _context.Pharmacies.Update(pharmacy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}