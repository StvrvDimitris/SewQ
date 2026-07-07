namespace SewQ.Services.Interfaces;

public interface IDatabaseInitializer
{
    /// <summary>Applies pending EF Core migrations. Called once at app startup.</summary>
    void Initialize();
}
