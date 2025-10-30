using System;
using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

/// <summary>
/// A no-op fallback speech service for non-Windows platforms or when speech is not available.
/// </summary>
public class SpeechService : ISpeechService
{
    public event Action<string>? PartialResult;
    public event Action<string>? FinalResult;

    public Task StartRecognitionAsync() => Task.CompletedTask;
    public Task StopRecognitionAsync() => Task.CompletedTask;
}
