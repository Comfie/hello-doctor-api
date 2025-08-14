Minimal clean-architecture and event-driven improvements

What changed (high level)
- Prescription: Added AssignedPharmacyId, simple guarded methods for Assign/Dispense/Deliver
- Domain events: Assigned/Dispensed/Delivered with old/new status
- Auditing: Added AuditLog entity; event handlers append StatusHistory + AuditLog
- JWT: Adds pharmacyId claim for Pharmacists for scoping
- Queries: pharmacy prescriptions list, counts by status, recent activity

EF Core migrations (local)
- dotnet tool restore
- dotnet ef migrations add AddMinimalEventsAndAudit -p src/Infrastructure -s src/Web
- dotnet ef database update -p src/Infrastructure -s src/Web

Endpoints you can wire in controllers
- POST api/v1/prescriptions/{id}/assign/{pharmacyId}
- POST api/v1/prescriptions/{id}/complete-dispense (body: { isPartial, note })
- POST api/v1/prescriptions/{id}/deliver
- GET  api/v1/pharmacies/{id}/prescriptions
- GET  api/v1/pharmacies/{id}/reports/counts
- GET  api/v1/pharmacies/{id}/reports/recent-activity?limit=20

Next small steps
- Add controller actions that call the new commands/queries
- Add 4–6 unit tests on Prescription transitions and handlers
- Add simple auth policy: Require role Pharmacist or SystemAdministrator for pharmacy-bound actions

