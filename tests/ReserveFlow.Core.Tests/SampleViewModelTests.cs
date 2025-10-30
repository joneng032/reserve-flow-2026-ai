using ReserveFlow.Core.ViewModels;
using Xunit;

namespace ReserveFlow.Core.Tests;

public class SampleViewModelTests
{
    [Fact]
    public void CounterStartsAtZero()
    {
        var vm = new SampleViewModel();
        Assert.Equal(0, vm.Counter);
    }

    [Fact]
    public void IncrementIncreasesCounter()
    {
        var vm = new SampleViewModel();
        vm.Increment();
        Assert.Equal(1, vm.Counter);
    }

    [Fact]
    public void DefaultTitleIsReserveFlow()
    {
        var vm = new SampleViewModel();
        Assert.Equal("Reserve Flow", vm.Title);
    }
}
