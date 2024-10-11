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
        const string Prefix = "CT";
        const int NumberLength = 4; // We want "CT" + 4 digits = 6 characters

        // Get the highest existing member number
        string? lastMemberNumber = await GetLastMemberNumberAsync(cancellationToken);

        int newNumber = 1;

        if (!string.IsNullOrEmpty(lastMemberNumber) && lastMemberNumber.StartsWith(Prefix))
        {
            // Extract the numeric part from the last member number
            string numberPart = lastMemberNumber.Substring(Prefix.Length);

            // Parse and increment it
            if (int.TryParse(numberPart, out int currentNumber))
            {
                newNumber = currentNumber + 1;
            }
        }

        // Generate the new member number, ensuring it is 4 digits
        string newMemberNumber = $"{Prefix}{newNumber.ToString().PadLeft(NumberLength, '0')}";

        return newMemberNumber;
    }
}