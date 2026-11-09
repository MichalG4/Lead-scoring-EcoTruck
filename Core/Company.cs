using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace LeadScoring.Core;

/// <summary>One row of the table = one company. Raises PropertyChanged so the UI (and the score) update instantly.</summary>
public sealed class Company : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private CompanyProfile _profile;
    private FleetSizeBand _fleetSize;
    private VehicleMix _vehicleType;
    private InterestLevel _interest;

    /// <summary>Stable key; will later let us match a row with its HubSpot company.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get => _name; set => Set(ref _name, value); }

    public CompanyProfile Profile { get => _profile; set => SetCriterion(ref _profile, value); }
    public FleetSizeBand FleetSize { get => _fleetSize; set => SetCriterion(ref _fleetSize, value); }
    public VehicleMix VehicleType { get => _vehicleType; set => SetCriterion(ref _vehicleType, value); }
    public InterestLevel Interest { get => _interest; set => SetCriterion(ref _interest, value); }

    [JsonIgnore]
    public int LeadScore => ScoringModel.Calculate(Profile, FleetSize, VehicleType, Interest);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void SetCriterion<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeadScore)));
    }
}
