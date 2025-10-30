using ReserveFlow.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public class ProjectsService : IProjectsService
{
    // Minimal in-memory sample implementation for scaffold/testing.
    public Task<IEnumerable<Project>> GetProjectsAsync()
    {
        var list = new List<Project>
        {
            new Project { Id = "p1", Title = "Runway Inspection", Description = "Field inspections near sector 7" },
            new Project { Id = "p2", Title = "Bridge Survey", Description = "Bridge structural inspection" }
        };

        return Task.FromResult<IEnumerable<Project>>(list);
    }
}
