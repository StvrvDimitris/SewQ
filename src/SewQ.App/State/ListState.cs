using SewQ.Services.Models;

namespace SewQ.App.State;

/// <summary>Keeps the list screen's active filter alive across navigation.</summary>
public sealed class ListState
{
    public ProjectStatus? Filter { get; set; }
}
