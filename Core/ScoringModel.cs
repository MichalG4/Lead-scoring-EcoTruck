using LeadScoring.Localization;

namespace LeadScoring.Core;

/// <summary>One dropdown entry: the value, its label and the points it is worth.</summary>
public sealed class Option<T>
{
    public Option(T value, string label, int? points = null)
    {
        Value = value;
        Label = label;
        Points = points ?? 0;
        Display = points is null ? label : $"{label} ({points} {Loc.T("PointsShort")})";
    }

    public T Value { get; }
    public string Label { get; }
    public int Points { get; }
    public string Display { get; }
}

/// <summary>
/// Single source of truth for the scoring rules (the table from the user story).
/// Dropdowns in the UI and the score calculation both read from these lists,
/// so changing a point value here changes it everywhere.
/// </summary>
public static class ScoringModel
{
    public static IReadOnlyList<Option<CompanyProfile>> ProfileOptions { get; } = new[]
    {
        new Option<CompanyProfile>(CompanyProfile.NotSelected, Loc.T("NotSelected")),
        new Option<CompanyProfile>(CompanyProfile.OwnFleet, Loc.T("Profile_OwnFleet"), 30),
        new Option<CompanyProfile>(CompanyProfile.Forwarding, Loc.T("Profile_Forwarding"), 10),
        new Option<CompanyProfile>(CompanyProfile.Other, Loc.T("Profile_Other"), 0),
    };

    public static IReadOnlyList<Option<FleetSizeBand>> FleetSizeOptions { get; } = new[]
    {
        new Option<FleetSizeBand>(FleetSizeBand.NotSelected, Loc.T("NotSelected")),
        new Option<FleetSizeBand>(FleetSizeBand.Over50, Loc.T("Fleet_Over50"), 25),
        new Option<FleetSizeBand>(FleetSizeBand.From10To50, Loc.T("Fleet_10to50"), 15),
        new Option<FleetSizeBand>(FleetSizeBand.Under10, Loc.T("Fleet_Under10"), 10),
    };

    public static IReadOnlyList<Option<VehicleMix>> VehicleTypeOptions { get; } = new[]
    {
        new Option<VehicleMix>(VehicleMix.NotSelected, Loc.T("NotSelected")),
        new Option<VehicleMix>(VehicleMix.TrucksOnly, Loc.T("Vehicle_TrucksOnly"), 25),
        new Option<VehicleMix>(VehicleMix.Mixed, Loc.T("Vehicle_Mixed"), 15),
        new Option<VehicleMix>(VehicleMix.MostlyVans, Loc.T("Vehicle_MostlyVans"), 5),
    };

    public static IReadOnlyList<Option<InterestLevel>> InterestOptions { get; } = new[]
    {
        new Option<InterestLevel>(InterestLevel.NotSelected, Loc.T("NotSelected")),
        new Option<InterestLevel>(InterestLevel.DemoRequested, Loc.T("Interest_Demo"), 20),
        new Option<InterestLevel>(InterestLevel.PositiveTalk, Loc.T("Interest_Positive"), 10),
        new Option<InterestLevel>(InterestLevel.AverageTalk, Loc.T("Interest_Average"), 0),
    };

    /// <summary>30 + 25 + 25 + 20 = 100.</summary>
    public static int MaxScore { get; } =
        ProfileOptions.Max(o => o.Points) +
        FleetSizeOptions.Max(o => o.Points) +
        VehicleTypeOptions.Max(o => o.Points) +
        InterestOptions.Max(o => o.Points);

    /// <summary>Criteria that are not selected yet are worth 0 points.</summary>
    public static int Calculate(CompanyProfile profile, FleetSizeBand fleetSize, VehicleMix vehicleType, InterestLevel interest) =>
        PointsOf(ProfileOptions, profile) +
        PointsOf(FleetSizeOptions, fleetSize) +
        PointsOf(VehicleTypeOptions, vehicleType) +
        PointsOf(InterestOptions, interest);

    private static int PointsOf<T>(IReadOnlyList<Option<T>> options, T value) =>
        options.FirstOrDefault(o => EqualityComparer<T>.Default.Equals(o.Value, value))?.Points ?? 0;
}
