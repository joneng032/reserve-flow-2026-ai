using System;
using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public interface IAudioService
{
    /// <summary>
    /// Start a recording session. The implementation should request permissions and prepare resources.
    /// </summary>
    Task StartRecordingAsync();

    /// <summary>
    /// Stop recording and return the saved file path and duration.
    /// </summary>
    Task<(string? FilePath, TimeSpan Duration)> StopRecordingAsync();

    /// <summary>
    /// Play the recording at the given file path (if available).
    /// </summary>
    Task PlayAsync(string filePath);
}
