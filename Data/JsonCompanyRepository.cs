using System.Text.Json;
using System.Text.Json.Serialization;
using LeadScoring.Core;
using System.IO;

namespace LeadScoring.Data;

public interface ICompanyRepository
{
    IReadOnlyList<Company> Load();
    void Save(IEnumerable<Company> companies);
}

/// <summary>
/// Stores the company list as JSON in %AppData%\LeadScoring\companies.json.
/// Swap for a SQLite implementation later by implementing ICompanyRepository.
/// </summary>
public sealed class JsonCompanyRepository : ICompanyRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly string _path;

    public JsonCompanyRepository(string? path = null)
    {
        _path = path ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LeadScoring",
            "companies.json");
    }

    public IReadOnlyList<Company> Load()
    {
        if (!File.Exists(_path)) return new List<Company>();

        try
        {
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Company>>(json, Options) ?? new List<Company>();
        }
        catch (JsonException)
        {
            // Keep the unreadable file instead of overwriting it with an empty list on the next save.
            File.Move(_path, $"{_path}.corrupt-{DateTime.Now:yyyyMMddHHmmss}");
            return new List<Company>();
        }
    }

    public void Save(IEnumerable<Company> companies)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(companies.ToList(), Options));
        File.Move(temp, _path, overwrite: true);
    }
}
