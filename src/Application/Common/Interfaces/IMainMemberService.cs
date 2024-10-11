namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IMainMemberService
{
   Task<string?> GetLastMemberNumberAsync(CancellationToken cancellationToken = default);
   Task<string> GenerateUniqueMemberNumberAsync(CancellationToken cancellationToken = default);
}