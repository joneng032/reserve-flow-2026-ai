using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public class CameraService : ICameraService
{
    public Task<byte[]?> CapturePhotoAsync()
    {
        // Minimal stub implementation that returns null. Platform-specific implementations
        // should be provided to actually capture photos on Android/iOS/Windows.
        return Task.FromResult<byte[]?>(null);
    }
}
