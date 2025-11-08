namespace HelloDoctorApi.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    long? GetPharmacyId();
    long? GetMainMemberId();
    long? GetDoctorId();
}