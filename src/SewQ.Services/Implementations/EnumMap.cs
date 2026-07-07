using SewQ.Services.Models;

namespace SewQ.Services.Implementations;

/// <summary>
/// Translates between contract enums and the strings stored in the database
/// (entities are deliberately stringly-typed so SewQ.Data has no dependency
/// on the contracts assembly).
/// </summary>
internal static class EnumMap
{
    public static ProjectStatus ToStatus(string value)
        => Enum.TryParse<ProjectStatus>(value, out var status) ? status : ProjectStatus.NotStarted;

    public static SewQTheme ToTheme(string value)
        => Enum.TryParse<SewQTheme>(value, out var theme) ? theme : SewQTheme.Atelier;

    public static Currency ToCurrency(string value)
        => Enum.TryParse<Currency>(value, out var currency) ? currency : Currency.EUR;
}
