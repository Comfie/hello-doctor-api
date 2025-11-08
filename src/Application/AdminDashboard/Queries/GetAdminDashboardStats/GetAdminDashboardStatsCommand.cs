using Ardalis;
using Ardalis.Result;
using HelloDoctorApi.Application.AdminDashboard.Models;
using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.AdminDashboard.Queries.GetAdminDashboardStats;

public record GetAdminDashboardStatsCommand() : IRequest<Result<AdminDashboardStatsResponse>>;

public class
    GetAdminDashboardStatsCommandHandler : IRequestHandler<GetAdminDashboardStatsCommand,
    Result<AdminDashboardStatsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetAdminDashboardStatsCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<AdminDashboardStatsResponse>> Handle(GetAdminDashboardStatsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var pharmacyId = _user.GetPharmacyId();

            // Execute all count queries - scoped by pharmacy if SystemAdmin
            var totalUsersTask = pharmacyId.HasValue
                ? await _context.ApplicationUsers
                    .AsNoTracking()
                    .Where(u => _context.Pharmacists.Any(p => p.AccountId == u.Id && p.PharmacyId == pharmacyId.Value) ||
                                _context.SystemAdministrators.Any(sa => sa.UserId == u.Id && sa.PharmacyId == pharmacyId.Value))
                    .CountAsync(cancellationToken)
                : await _context.ApplicationUsers
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

            var activeUsersTask = pharmacyId.HasValue
                ? await _context.ApplicationUsers
                    .AsNoTracking()
                    .Where(u => u.IsActive && !u.IsDeleted &&
                                (_context.Pharmacists.Any(p => p.AccountId == u.Id && p.PharmacyId == pharmacyId.Value) ||
                                 _context.SystemAdministrators.Any(sa => sa.UserId == u.Id && sa.PharmacyId == pharmacyId.Value)))
                    .CountAsync(cancellationToken)
                : await _context.ApplicationUsers
                    .AsNoTracking()
                    .CountAsync(u => u.IsActive && !u.IsDeleted, cancellationToken);

            // For SystemAdmin, show beneficiaries from prescriptions in their pharmacy
            var totalBeneficiariesTask = pharmacyId.HasValue
                ? await _context.Prescriptions
                    .AsNoTracking()
                    .Where(p => p.AssignedPharmacyId == pharmacyId.Value)
                    .Select(p => p.BeneficiaryId)
                    .Distinct()
                    .CountAsync(cancellationToken)
                : await _context.Beneficiaries
                    .AsNoTracking()
                    .CountAsync(b => !b.IsDeleted, cancellationToken);

            // For SystemAdmin, show doctors associated with their pharmacy
            var totalDoctorsTask = pharmacyId.HasValue
                ? await _context.Doctors
                    .AsNoTracking()
                    .Where(d => d.Pharmacies!.Any(p => p.Id == pharmacyId.Value))
                    .CountAsync(cancellationToken)
                : await _context.Doctors
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

            var activeDoctorsTask = pharmacyId.HasValue
                ? await _context.Doctors
                    .AsNoTracking()
                    .Where(d => d.IsActive && d.Pharmacies!.Any(p => p.Id == pharmacyId.Value))
                    .CountAsync(cancellationToken)
                : await _context.Doctors
                    .AsNoTracking()
                    .CountAsync(d => d.IsActive, cancellationToken);

            // For SystemAdmin, show only their pharmacy
            var totalPharmaciesTask = pharmacyId.HasValue
                ? 1
                : await _context.Pharmacies
                    .AsNoTracking()
                    .CountAsync(p => !p.IsDeleted, cancellationToken);

            var activePharmaciesTask = pharmacyId.HasValue
                ? await _context.Pharmacies
                    .AsNoTracking()
                    .CountAsync(p => p.Id == pharmacyId.Value && p.IsActive && !p.IsDeleted, cancellationToken)
                : await _context.Pharmacies
                    .AsNoTracking()
                    .CountAsync(p => p.IsActive && !p.IsDeleted, cancellationToken);

            // For SystemAdmin, show main members who have prescriptions at their pharmacy
            var totalMainMembersTask = pharmacyId.HasValue
                ? await _context.Prescriptions
                    .AsNoTracking()
                    .Where(p => p.AssignedPharmacyId == pharmacyId.Value)
                    .Select(p => p.MainMemberId)
                    .Distinct()
                    .CountAsync(cancellationToken)
                : await _context.MainMembers
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

            var activeMainMembersTask = pharmacyId.HasValue
                ? await (from p in _context.Prescriptions.AsNoTracking()
                         join mm in _context.MainMembers.AsNoTracking() on p.MainMemberId equals mm.AccountId
                         join u in _context.ApplicationUsers.AsNoTracking() on mm.AccountId equals u.Id
                         where p.AssignedPharmacyId == pharmacyId.Value && u.IsActive
                         select mm.Id)
                    .Distinct()
                    .CountAsync(cancellationToken)
                : await _context.MainMembers
                    .AsNoTracking()
                    .Include(m => m.Account)
                    .CountAsync(m => m.Account.IsActive, cancellationToken);

            // For SystemAdmin, show prescriptions for their pharmacy
            var totalPrescriptionsTask = pharmacyId.HasValue
                ? await _context.Prescriptions
                    .AsNoTracking()
                    .Where(p => p.AssignedPharmacyId == pharmacyId.Value)
                    .CountAsync(cancellationToken)
                : await _context.Prescriptions
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

            var prescriptionsByStatusTask = pharmacyId.HasValue
                ? await _context.Prescriptions
                    .AsNoTracking()
                    .Where(p => p.AssignedPharmacyId == pharmacyId.Value)
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToListAsync(cancellationToken)
                : await _context.Prescriptions
                    .AsNoTracking()
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToListAsync(cancellationToken);

            // Wait for all tasks to complete

            var prescriptionsByStatus = prescriptionsByStatusTask
                .ToDictionary(x => x.Status, x => x.Count);

            var stats = new AdminDashboardStatsResponse
            {
                TotalUsers = totalUsersTask,
                TotalActiveUsers = activeUsersTask,
                TotalBeneficiaries = totalBeneficiariesTask,
                TotalDoctors = totalDoctorsTask,
                TotalDoctorsActive = activeDoctorsTask,
                TotalPharmacies = totalPharmaciesTask,
                TotalActivePharmacies = activePharmaciesTask,
                TotalMainMembers = totalMainMembersTask,
                TotalActiveMainMembers = activeMainMembersTask,
                TotalPrescriptions = totalPrescriptionsTask,
                PendingPrescriptions = prescriptionsByStatus.GetValueOrDefault("Pending", 0),
                CompletedPrescriptions = prescriptionsByStatus.GetValueOrDefault("Delivered", 0),
                PrescriptionsByStatus = prescriptionsByStatus
            };

            return Result.Success(stats);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to retrieve admin dashboard stats: {ex.Message}");
        }
    }
}