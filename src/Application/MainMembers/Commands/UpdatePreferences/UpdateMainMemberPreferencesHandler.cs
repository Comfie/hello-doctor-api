using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.MainMembers.Commands.UpdatePreferences;

public class UpdateMainMemberPreferencesHandler : IRequestHandler<UpdateMainMemberPreferencesCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public UpdateMainMemberPreferencesHandler(IApplicationDbContext db, IUser user)
    { _db = db; _user = user; }

    public async Task<Result<bool>> Handle(UpdateMainMemberPreferencesCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return Result<bool>.Unauthorized();

        var member = await _db.MainMembers.FirstOrDefaultAsync(m => m.AccountId == _user.Id, ct);
        if (member is null) return Result<bool>.NotFound();

        member.DefaultPharmacyId = request.DefaultPharmacyId;
        await _db.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}

