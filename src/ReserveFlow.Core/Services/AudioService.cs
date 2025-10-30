using System;
using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public class AudioService : IAudioService
{
    public Task StartRecordingAsync()
    {
        // Stub: platform implementations must replace this. Could initialize recorder, request permissions, etc.
        return Task.CompletedTask;
    }

    public Task<(string? FilePath, TimeSpan Duration)> StopRecordingAsync()
    {
        // Stub: return null to indicate no recording in this scaffold.
        return Task.FromResult<(string?, TimeSpan)>((null, TimeSpan.Zero));
    }

    public Task PlayAsync(string filePath)
    {
        // Stub: no-op in scaffold.
        return Task.CompletedTask;
    }
}
