namespace SewQ.App;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new MainPage()) { Title = "SewQ" };
#if WINDOWS
		// Phone-first design; open in a phone-shaped window on desktop.
		window.Width = 440;
		window.Height = 920;
#endif
		return window;
	}
}
