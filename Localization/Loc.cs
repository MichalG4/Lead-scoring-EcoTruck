using System.ComponentModel;

namespace LeadScoring.Localization;

/// <summary>
/// Minimal localization. XAML binds to the indexer:
///   {Binding [AddCompany], Source={x:Static loc:Loc.Instance}}
/// To add English later: add an "en" dictionary below (same keys) and set
/// Loc.Instance.Language = "en" at startup. Missing keys fall back to Polish.
/// Note: dropdown labels are built once at startup, so switch the language before MainWindow opens.
/// </summary>
public sealed class Loc : INotifyPropertyChanged
{
    public static Loc Instance { get; } = new();

    private static readonly Dictionary<string, Dictionary<string, string>> Languages = new()
    {
        ["pl"] = new()
        {
            ["AppTitle"] = "Lead Scoring – priorytetyzacja leadów",
            ["AddCompany"] = "Dodaj firmę",
            ["DeleteSelected"] = "Usuń zaznaczone",
            ["AutoSort"] = "Sortuj automatycznie wg Lead Score",
            ["MaxScoreInfo"] = "Maks. Lead Score: {0} pkt",

            ["ColCompany"] = "Firma",
            ["ColProfile"] = "Profil firmy",
            ["ColFleetSize"] = "Wielkość floty",
            ["ColVehicleType"] = "Typ pojazdów",
            ["ColInterest"] = "Zainteresowanie",
            ["ColScore"] = "Lead Score",

            ["NewCompanyName"] = "Nowa firma",
            ["NotSelected"] = "— wybierz —",
            ["PointsShort"] = "pkt",

            ["Profile_OwnFleet"] = "Transport z własną flotą",
            ["Profile_Forwarding"] = "Spedycja",
            ["Profile_Other"] = "Inny",

            ["Fleet_Over50"] = "Powyżej 50 pojazdów",
            ["Fleet_10to50"] = "10–50 pojazdów",
            ["Fleet_Under10"] = "Poniżej 10 pojazdów",

            ["Vehicle_TrucksOnly"] = "Tylko ciężarówki",
            ["Vehicle_Mixed"] = "Mieszane",
            ["Vehicle_MostlyVans"] = "Głównie dostawcze",

            ["Interest_Demo"] = "Chęć demo",
            ["Interest_Positive"] = "Pozytywna rozmowa",
            ["Interest_Average"] = "Średnia rozmowa",

            ["ConfirmDeleteTitle"] = "Potwierdzenie",
            ["ConfirmDeleteText"] = "Usunąć zaznaczone firmy ({0})?",
            ["SaveError"] = "Nie udało się zapisać danych:\n{0}",
            ["LoadError"] = "Nie udało się wczytać danych:\n{0}",
        },
        // ["en"] = new() { ["AddCompany"] = "Add company", ... },
    };

    private string _language = "pl";

    public string Language
    {
        get => _language;
        set
        {
            if (value == _language || !Languages.ContainsKey(value)) return;
            _language = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }

    public string this[string key]
    {
        get
        {
            if (Languages[_language].TryGetValue(key, out var text)) return text;
            if (Languages["pl"].TryGetValue(key, out text)) return text;
            return key;
        }
    }

    public static string T(string key) => Instance[key];

    public event PropertyChangedEventHandler? PropertyChanged;
}
