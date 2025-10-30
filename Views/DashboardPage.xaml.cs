using ReserveFlow.Core.ViewModels;
using System.Linq;

namespace reserve_flow_ai_2026;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;

    // ViewModel will be provided by DI when the page is created via the service provider.
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = _vm;

        RefreshBtn.Clicked += async (_, __) => await _vm.LoadProjectsAsync();

        ProjectsList.ItemsSource = _vm.Projects;
    }
}
