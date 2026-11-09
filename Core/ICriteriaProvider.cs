namespace LeadScoring.Core;

/// <summary>
/// Seam for version 2 (not wired into the UI yet). A HubSpot implementation will fill
/// Profile / FleetSize / VehicleType from HubSpot company properties.
/// Interest is deliberately absent: it always stays a manual choice.
/// A null field means "HubSpot has no (mappable) value" and leaves the manual choice untouched.
/// </summary>
public sealed record CrmCriteria(
    CompanyProfile? Profile,
    FleetSizeBand? FleetSize,
    VehicleMix? VehicleType);

public interface ICriteriaProvider
{
    Task<CrmCriteria?> GetCriteriaAsync(Company company, CancellationToken cancellationToken = default);
}
