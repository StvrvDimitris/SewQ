using System.Globalization;
using SewQ.Services.Models;

namespace SewQ.App.State;

public static class UiText
{
    public static string StatusCss(ProjectStatus status) => status switch
    {
        ProjectStatus.InProgress => "status-progress",
        ProjectStatus.Finished => "status-finished",
        ProjectStatus.Abandoned => "status-abandoned",
        _ => "status-new",
    };

    /// <summary>"just now" / "today" / "yesterday" / "3 days ago" / "12 Mar 2026".</summary>
    public static string Relative(DateTime utc)
    {
        var local = utc.ToLocalTime();
        var now = DateTime.Now;
        if (now - local < TimeSpan.FromMinutes(1))
            return "just now";
        if (local.Date == now.Date)
            return "today";
        var days = (now.Date - local.Date).Days;
        if (days == 1)
            return "yesterday";
        if (days < 7)
            return $"{days} days ago";
        return local.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Tolerant decimal parse for form fields: accepts both "3.5" and "3,5",
    /// returns 0 for anything unparsable — same forgiving behaviour as the
    /// prototype's parseFloat(...) || 0.
    /// </summary>
    public static decimal ParseDecimal(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0m;
        return decimal.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0m;
    }
}
