using Microsoft.Extensions.Logging;

namespace reserve_flow_ai_2026;

public static class MauiProgram
{
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
	builder.Services.AddSingleton<ReserveFlow.Core.Services.IProjectsService, ReserveFlow.Core.Services.ProjectsService>();
	builder.Services.AddSingleton<ReserveFlow.Core.ViewModels.DashboardViewModel>();

	return builder.Build();
	}
}
