using System;

namespace ReserveFlow.Core.Models;

public class GeotaggedAudioAttachment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? FilePath { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Transcription { get; set; }
    public string? ProjectId { get; set; }
}
