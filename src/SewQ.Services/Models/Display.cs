using System.Globalization;

namespace SewQ.Services.Models;

public static class Display
{
    public static string Label(this ProjectStatus status) => status switch
    {
        ProjectStatus.InProgress => "In progress",
        ProjectStatus.Finished => "Finished",
        ProjectStatus.Abandoned => "Abandoned",
        _ => "Not started",
    };

    public static string Label(this SewQTheme theme) => theme switch
    {
        SewQTheme.CraftRoom => "Craft Room",
        SewQTheme.Studio => "Studio",
        _ => "Atelier",
    };

    public static string Symbol(this Currency currency) => currency switch
    {
        Currency.USD => "$",
        Currency.GBP => "£",
        _ => "€",
    };

    /// <summary>Formats like the design: symbol immediately followed by 2 decimals, e.g. "€14.00".</summary>
    public static string Money(this decimal amount, Currency currency)
        => currency.Symbol() + amount.ToString("0.00", CultureInfo.InvariantCulture);

    /// <summary>Compact number without trailing zeros, e.g. hours "0.5" or "3".</summary>
    public static string Compact(this decimal value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);
}
