using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Infrastructure.Services;

public class MainMainMemberService : IMainMemberService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public MainMainMemberService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<string?> GetLastMemberNumberAsync(CancellationToken cancellationToken = default)
    {
        return await _applicationDbContext.MainMembers
            .OrderByDescending(m => m.Code)
            .Select(m => m.Code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string> GenerateUniqueMemberNumberAsync(CancellationToken cancellationToken = default)
    {
        const string prefix = "CT";
        const int numberLength = 4; // We want "CT" + 4 digits = 6 characters

        // Get the highest existing member number
        var lastMemberNumber = await _applicationDbContext.MainMembers
            .OrderByDescending(m => m.Code)
            .Select(m => m.Code)
            .FirstOrDefaultAsync(cancellationToken);

        var newNumber = 1;

        if (!string.IsNullOrEmpty(lastMemberNumber) && lastMemberNumber.StartsWith(prefix))
        {
            var numberPart = lastMemberNumber.Substring(prefix.Length);

            // Parse and increment it
            if (int.TryParse(numberPart, out var currentNumber))
            {
                newNumber = currentNumber + 1;
            }
        }

        var newMemberNumber = $"{prefix}{newNumber.ToString().PadLeft(numberLength, '0')}";

        return newMemberNumber;
    }
}