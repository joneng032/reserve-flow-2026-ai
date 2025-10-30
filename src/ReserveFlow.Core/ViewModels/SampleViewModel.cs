namespace ReserveFlow.Core.ViewModels;

public class SampleViewModel
{
    public string Title { get; set; } = "Reserve Flow";

    public int Counter { get; private set; }

    public void Increment() => Counter++;
}
