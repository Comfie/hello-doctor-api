namespace HelloDoctorApi.Application.Prescriptions.Models;

public record CompleteDispenseDto(bool IsPartial, string? Note);
