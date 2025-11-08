using FluentAssertions;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Tests;

public class PrescriptionAggregateTests
{
    [Fact]
    public void AssignToPharmacy_SetsAssignedAndRaisesEvent_WhenAllowed()
    {
        var p = NewPendingPrescription();

        p.AssignToPharmacy(5);

        p.AssignedPharmacyId.Should().Be(5);
        p.Status.Should().Be(PrescriptionStatus.UnderReview);
        p.DomainEvents.Should().NotBeEmpty();
    }

    [Fact]
    public void CompleteDispense_SetsPartial_WhenAllowed()
    {
        var p = NewPendingPrescription();
        p.AssignToPharmacy(2);

        p.CompleteDispense(isPartial: true, note: "shortage");

        p.Status.Should().Be(PrescriptionStatus.PartiallyDispensed);
    }

    [Fact]
    public void MarkDelivered_SetsDelivered_WhenDispensed()
    {
        var p = NewPendingPrescription();
        p.AssignToPharmacy(2);
        p.CompleteDispense(isPartial: false);

        p.MarkDelivered();

        p.Status.Should().Be(PrescriptionStatus.Delivered);
    }

    private static Prescription NewPendingPrescription()
        => new()
        {
            IssuedDate = DateTimeOffset.UtcNow,
            ExpiryDate = DateTimeOffset.UtcNow.AddDays(7),
            Status = PrescriptionStatus.Pending,
            MainMemberId = "user-1",
            MainMember = new Domain.Entities.Auth.ApplicationUser { Id = "user-1" },
            BeneficiaryId = 1,
            Beneficiary = new Beneficiary
            {
                FirstName = "John", LastName = "Doe", PhoneNumber = "123",
                EmailAddress = "j@d.com", BeneficiaryCode = "B1",
                Relationship = RelationshipToMainMember.Child,
                MainMemberId = "user-1",
                MainMember = new Domain.Entities.Auth.ApplicationUser { Id = "user-1" },
                Gender = "male",
                DateOfBirth = DateTime.Now.AddYears(-18)
            }
        };
}

