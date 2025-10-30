using System;
using System.Threading.Tasks;

namespace ReserveFlow.Core.Services;

public interface ISpeechService
{
    /// <summary>
    /// Raised when a partial/transient transcription is available (low-latency hypothesis).
    /// </summary>
    event Action<string>? PartialResult;

    /// <summary>
    /// Raised when a final transcription segment is available.
    /// </summary>
    event Action<string>? FinalResult;

    Task StartRecognitionAsync();
    Task StopRecognitionAsync();
}
