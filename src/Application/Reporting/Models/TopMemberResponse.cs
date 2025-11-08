namespace HelloDoctorApi.Application.Reporting.Models;


public record TopMemberResponse(
    string MainMemberId,
    string MainMemberName,
    string MemberCode,
    int TotalPrescriptions,
    int PendingPrescriptions,
    int CompletedPrescriptions,
    DateTimeOffset? LastPrescriptionDate,
    string Email,
    string PhoneNumber
 
);
