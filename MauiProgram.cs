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
		// Core services
		builder.Services.AddTransient<ReserveFlow.Core.Services.IProjectsService, ReserveFlow.Core.Services.ProjectsService>();
		builder.Services.AddTransient<ReserveFlow.Core.Services.ICameraService, ReserveFlow.Core.Services.CameraService>();
		builder.Services.AddSingleton<ReserveFlow.Core.Services.IStorageService, ReserveFlow.Core.Services.StorageService>();

		// Platform-specific registrations
		// Platform-specific registrations
		// Use compile-time guards so non-Windows TFMs don't need to resolve Windows-only types.
		#if WINDOWS
		builder.Services.AddSingleton<ReserveFlow.Core.Services.IAudioService, reserve_flow_ai_2026.Platforms.Windows.WindowsAudioService>();
		builder.Services.AddSingleton<ReserveFlow.Core.Services.ISpeechService, reserve_flow_ai_2026.Platforms.Windows.WindowsSpeechService>();
		#else
		builder.Services.AddSingleton<ReserveFlow.Core.Services.IAudioService, ReserveFlow.Core.Services.AudioService>();
		builder.Services.AddSingleton<ReserveFlow.Core.Services.ISpeechService, ReserveFlow.Core.Services.SpeechService>();
		#endif

		// ViewModels
		builder.Services.AddTransient<ReserveFlow.Core.ViewModels.DashboardViewModel>();
		builder.Services.AddTransient<ReserveFlow.Core.ViewModels.LoginViewModel>();
		builder.Services.AddTransient<ReserveFlow.Core.ViewModels.ProjectDetailViewModel>();

		// Pages
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<ProjectDetailPage>();
		builder.Services.AddTransient<RecordingPage>();
		builder.Services.AddTransient<ReserveFlow.Core.ViewModels.RecordingViewModel>();

		var app = builder.Build();
		Services = app.Services;

		return app;
	}
}
