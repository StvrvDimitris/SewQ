namespace SewQ.Services.Models;

public enum ProjectStatus
{
    NotStarted,
    InProgress,
    Finished,
    Abandoned,
}

// Not named "AppTheme" to avoid clashing with Microsoft.Maui.ApplicationModel.AppTheme
// in the UI project, where MAUI usings are global.
public enum SewQTheme
{
    Atelier,
    CraftRoom,
    Studio,
}

public enum Currency
{
    EUR,
    USD,
    GBP,
}
