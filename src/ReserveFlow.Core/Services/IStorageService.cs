using System.Threading.Tasks;
using ReserveFlow.Core.Models;

namespace ReserveFlow.Core.Services;

public interface IStorageService
{
    Task InitializeAsync();
    Task SaveAttachmentAsync(GeotaggedAudioAttachment attachment);
}
