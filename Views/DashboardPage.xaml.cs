using ReserveFlow.Core.ViewModels;
using System.Linq;

namespace reserve_flow_ai_2026;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;

    public DashboardPage()
    {
        InitializeComponent();

        // Simple scaffold: instantiate ViewModel with default service.
        // We'll wire DI in MauiProgram later; for now create a concrete ProjectsService.
        _vm = new DashboardViewModel(new ReserveFlow.Core.Services.ProjectsService());
        BindingContext = _vm;

        RefreshBtn.Clicked += async (_, __) => await _vm.LoadProjectsAsync();

        ProjectsList.ItemsSource = _vm.Projects;
    }
}
