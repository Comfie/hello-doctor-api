namespace HelloDoctorApi.Application.Reporting.Models;

public class PerformanceMetricsResponse
{
    // Time-based metrics
    public double AverageTurnaroundHours { get; set; }
    public double MedianTurnaroundHours { get; set; }
    public double FastestTurnaroundHours { get; set; }
    public double SlowestTurnaroundHours { get; set; }

    // Completion rates
    public int TotalAssigned { get; set; }
    public int TotalCompleted { get; set; }
    public double CompletionRate { get; set; } // Percentage

    // Daily breakdown (last 7 days)
    public List<DailyMetric> DailyMetrics { get; set; } = new();

    // Status distribution over time
    public int CurrentlyInProgress { get; set; }
    public int CurrentlyOnHold { get; set; }
}

public record DailyMetric(
    DateOnly Date,
    int Received,
    int Completed,
    double AverageTurnaroundHours
);
