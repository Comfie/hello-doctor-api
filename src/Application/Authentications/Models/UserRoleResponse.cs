namespace HelloDoctorApi.Application.Authentications.Models;

public record UserRoleResponse
{
    public string Id { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public string Description { get; set; } = string.Empty;
}