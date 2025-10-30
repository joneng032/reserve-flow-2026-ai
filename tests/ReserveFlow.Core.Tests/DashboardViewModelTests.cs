using System.Threading.Tasks;
using ReserveFlow.Core.Services;
using ReserveFlow.Core.ViewModels;
using Xunit;

namespace ReserveFlow.Core.Tests;

public class DashboardViewModelTests
{
    [Fact]
    public async Task LoadProjects_PopulatesCollection()
    {
        var svc = new ProjectsService();
        var vm = new DashboardViewModel(svc);

        await vm.LoadProjectsAsync();

        Assert.NotEmpty(vm.Projects);
    }
}
