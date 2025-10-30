using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public interface ICameraService
{
    /// <summary>
    /// Capture a photo and return the image bytes, or null if cancelled/not available.
    /// </summary>
    Task<byte[]?> CapturePhotoAsync();
}
