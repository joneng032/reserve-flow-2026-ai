namespace reserve_flow_ai_2026;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// Replace Dashboard ShellContent's page with a DI-resolved instance so
		// the DashboardPage receives constructor-injected services/ViewModel.
		try
		{
			var dashboardShell = this.Items
				.SelectMany(i => i.Items)
				.OfType<ShellContent>()
				.FirstOrDefault(sc => sc.Route == "Dashboard");

			if (dashboardShell != null && MauiProgram.Services != null)
			{
				var page = MauiProgram.Services.GetService(typeof(DashboardPage)) as Page;
				if (page != null)
				{
					dashboardShell.Content = page;
					dashboardShell.ContentTemplate = null;
				}
			}
		}
		catch
		{
			// Swallow — DI wiring is best-effort here for scaffolding.
		}
	}
}
