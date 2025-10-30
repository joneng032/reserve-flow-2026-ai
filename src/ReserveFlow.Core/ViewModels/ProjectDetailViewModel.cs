using ReserveFlow.Core.Models;
using ReserveFlow.Core.Services;
using System.Threading.Tasks;

namespace ReserveFlow.Core.ViewModels;

public class ProjectDetailViewModel
{
    private readonly IProjectsService _projectsService;

    public Project? Project { get; private set; }

    public ProjectDetailViewModel(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    public Task LoadAsync(string projectId)
    {
        // For now, just load the list and pick the matching id as a simple placeholder.
        return Task.Run(async () =>
        {
            var items = await _projectsService.GetProjectsAsync();
            foreach (var p in items)
            {
                if (p.Id == projectId)
                {
                    Project = p;
                    break;
                }
            }
        });
    }
}
