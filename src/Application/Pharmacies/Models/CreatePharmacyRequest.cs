using Microsoft.AspNetCore.Http;

namespace ApiBaseTemplate.Application.Pharmacies.Models;

public class CreatePharmacyRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ContactNumber { get; set; }
    public required string ContactEmail { get; set; }
    public required string ContactPerson { get; set; }
    public required string Address { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
}