namespace HelloDoctorApi.Application.AdminDashboard.Models;

public class AdminDashboardStatsResponse
{
    public int TotalUsers { get; set; }
    public int TotalActiveUsers { get; set; }
    public int TotalBeneficiaries { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalDoctorsActive { get; set; }
    public int TotalPharmacies { get; set; }
    public int TotalActivePharmacies { get; set; }
    public int TotalMainMembers { get; set; }
    public int TotalActiveMainMembers { get; set; }
    public int TotalPrescriptions { get; set; }
    public int PendingPrescriptions { get; set; }
    public int CompletedPrescriptions { get; set; }
    public Dictionary<string, int> PrescriptionsByStatus { get; set; } = new();
}