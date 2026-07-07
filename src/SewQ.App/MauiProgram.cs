using Microsoft.Extensions.Logging;
using SewQ.App.State;
using SewQ.Services;
using SewQ.Services.Interfaces;

namespace SewQ.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>();

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var databasePath = Path.Combine(FileSystem.AppDataDirectory, "sewq.db3");
		builder.Services.AddSewQServices(databasePath);
		builder.Services.AddSingleton<ThemeState>();
		builder.Services.AddSingleton<ListState>();

		var app = builder.Build();
		app.Services.GetRequiredService<IDatabaseInitializer>().Initialize();
		return app;
	}
}
