using ReserveFlow.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public interface IProjectsService
{
    Task<IEnumerable<Project>> GetProjectsAsync();
}

