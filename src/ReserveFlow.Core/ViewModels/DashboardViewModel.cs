using ReserveFlow.Core.Models;
using ReserveFlow.Core.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ReserveFlow.Core.ViewModels;

public class DashboardViewModel
{
    private readonly IProjectsService _projectsService;

    public ObservableCollection<Project> Projects { get; } = new();

    public DashboardViewModel(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    public async Task LoadProjectsAsync()
    {
        var items = await _projectsService.GetProjectsAsync();
        Projects.Clear();
        foreach (var p in items)
            Projects.Add(p);
    }
}
