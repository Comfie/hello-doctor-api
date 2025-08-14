using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Events;
using HelloDoctorApi.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Events;

public class PrescriptionAssignedHandler : INotificationHandler<PrescriptionAssignedToPharmacyEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IDateTimeService _clock;

    public PrescriptionAssignedHandler(IApplicationDbContext db, IUser user, IDateTimeService clock)
    { _db = db; _user = user; _clock = clock; }

    public async Task Handle(PrescriptionAssignedToPharmacyEvent e, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == e.PrescriptionId, ct);
        if (pres is null) return;

        _db.PrescriptionStatusHistories.Add(new PrescriptionStatusHistory
        {
            PrescriptionId = pres.Id,
            OldStatus = e.OldStatus,
            NewStatus = e.NewStatus,
            ChangedByUserId = _user.Id ?? "system",
            ChangedDate = _clock.OffsetNow,
            Reason = $"Assigned to pharmacy {e.PharmacyId}",
            Prescription = pres
        });

        _db.AuditLogs.Add(new AuditLog
        {
            Action = "PrescriptionAssigned",
            ActorUserId = _user.Id,
            PrescriptionId = pres.Id,
            PharmacyId = e.PharmacyId,
            Details = $"Old={e.OldStatus}, New={e.NewStatus}"
        });

        await _db.SaveChangesAsync(ct);
    }
}

public class PrescriptionDispensedHandler : INotificationHandler<PrescriptionDispensedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IDateTimeService _clock;

    public PrescriptionDispensedHandler(IApplicationDbContext db, IUser user, IDateTimeService clock)
    { _db = db; _user = user; _clock = clock; }

    public async Task Handle(PrescriptionDispensedEvent e, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == e.PrescriptionId, ct);
        if (pres is null) return;

        _db.PrescriptionStatusHistories.Add(new PrescriptionStatusHistory
        {
            PrescriptionId = pres.Id,
            OldStatus = e.OldStatus,
            NewStatus = e.NewStatus,
            ChangedByUserId = _user.Id ?? "system",
            ChangedDate = _clock.OffsetNow,
            Reason = e.IsPartial ? "Partial dispense" : "Full dispense",
            Prescription = pres
        });

        _db.AuditLogs.Add(new AuditLog
        {
            Action = "PrescriptionDispensed",
            ActorUserId = _user.Id,
            PrescriptionId = pres.Id,
            PharmacyId = pres.AssignedPharmacyId,
            Details = $"IsPartial={e.IsPartial}; Note={e.Note}"
        });

        await _db.SaveChangesAsync(ct);
    }
}

public class PrescriptionDeliveredHandler : INotificationHandler<PrescriptionDeliveredEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IDateTimeService _clock;

    public PrescriptionDeliveredHandler(IApplicationDbContext db, IUser user, IDateTimeService clock)
    { _db = db; _user = user; _clock = clock; }

    public async Task Handle(PrescriptionDeliveredEvent e, CancellationToken ct)
    {
        var pres = await _db.Prescriptions.FirstOrDefaultAsync(x => x.Id == e.PrescriptionId, ct);
        if (pres is null) return;

        _db.PrescriptionStatusHistories.Add(new PrescriptionStatusHistory
        {
            PrescriptionId = pres.Id,
            OldStatus = e.OldStatus,
            NewStatus = e.NewStatus,
            ChangedByUserId = _user.Id ?? "system",
            ChangedDate = _clock.OffsetNow,
            Reason = "Delivered",
            Prescription = pres
        });

        _db.AuditLogs.Add(new AuditLog
        {
            Action = "PrescriptionDelivered",
            ActorUserId = _user.Id,
            PrescriptionId = pres.Id,
            PharmacyId = pres.AssignedPharmacyId,
        });

        await _db.SaveChangesAsync(ct);
    }
}

