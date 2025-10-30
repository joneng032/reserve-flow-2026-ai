using System.Threading.Tasks;
using System.Windows.Input;
using ReserveFlow.Core.Services;
using ReserveFlow.Core.Models;

namespace ReserveFlow.Core.ViewModels;

public class LoginViewModel
{
    private readonly IProjectsService _projectsService; // placeholder for injected services like IAuthService

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICommand? LoginCommand { get; set; }

    public LoginViewModel(IProjectsService projectsService)
    {
        _projectsService = projectsService;
        // Commands can be wired up by the view or later with a Command implementation
    }

    public Task<bool> LoginAsync()
    {
        // Placeholder implementation; integrate with an auth service later
        return Task.FromResult(!string.IsNullOrWhiteSpace(Username));
    }
}
