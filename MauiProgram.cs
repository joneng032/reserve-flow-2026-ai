using Microsoft.Extensions.Logging;

namespace reserve_flow_ai_2026;

public static class MauiProgram
{
	public static IServiceProvider? Services { get; private set; }

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register core services and viewmodels (scaffold)
		builder.Services.AddTransient<ReserveFlow.Core.Services.IProjectsService, ReserveFlow.Core.Services.ProjectsService>();
		builder.Services.AddTransient<ReserveFlow.Core.ViewModels.DashboardViewModel>();
		builder.Services.AddTransient<DashboardPage>();

		var app = builder.Build();
		Services = app.Services;

		return app;
	}
}
