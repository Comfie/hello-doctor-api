namespace HelloDoctorApi.Application.Reporting.Models;

public class PharmacyDashboardStatsResponse
{
    // Overall Metrics
    public int TotalPrescriptions { get; set; }
    public int TodaysPrescriptions { get; set; }
    public int ThisWeekPrescriptions { get; set; }
    public int ThisMonthPrescriptions { get; set; }

    // Status Breakdown
    public int PendingReview { get; set; }
    public int UnderReview { get; set; }
    public int OnHold { get; set; }
    public int ReadyForDispensing { get; set; }
    public int PartiallyDispensed { get; set; }
    public int FullyDispensed { get; set; }
    public int ReadyForPickup { get; set; }
    public int Delivered { get; set; }
    public Dictionary<string, int> PrescriptionsByStatus { get; set; } = new();

    // Urgent Actions Required
    public int RequiringAttention { get; set; } // Pending + OnHold
    public int OverduePrescriptions { get; set; } // Expired but not dispensed

    // Performance Metrics
    public double AverageTurnaroundHours { get; set; } // From assignment to dispensed
    public int CompletedToday { get; set; }
    public int CompletedThisWeek { get; set; }
    public int CompletedThisMonth { get; set; }

    // Top Statistics
    public int TotalMainMembers { get; set; } // Unique main members
    public int TotalBeneficiaries { get; set; } // Unique beneficiaries
}
