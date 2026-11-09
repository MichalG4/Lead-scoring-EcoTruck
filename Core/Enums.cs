namespace LeadScoring.Core;

// Numeric values are only used for sorting by column; persisted as strings in JSON.

public enum CompanyProfile
{
    NotSelected = 0,
    OwnFleet,    // Transport z własną flotą – 30 pkt
    Forwarding,  // Spedycja – 10 pkt
    Other,       // Inny – 0 pkt
}

public enum FleetSizeBand
{
    NotSelected = 0,
    Over50,      // Powyżej 50 – 25 pkt
    From10To50,  // 10–50 – 15 pkt
    Under10,     // Poniżej 10 – 10 pkt
}

public enum VehicleMix
{
    NotSelected = 0,
    TrucksOnly,  // Tylko ciężarówki – 25 pkt
    Mixed,       // Mieszane – 15 pkt
    MostlyVans,  // Głównie dostawcze – 5 pkt
}

public enum InterestLevel
{
    NotSelected = 0,
    DemoRequested, // Chęć demo – 20 pkt
    PositiveTalk,  // Pozytywna rozmowa – 10 pkt
    AverageTalk,   // Średnia rozmowa – 0 pkt
}
