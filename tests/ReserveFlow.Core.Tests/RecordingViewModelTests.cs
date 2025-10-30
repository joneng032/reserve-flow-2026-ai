using System.Threading.Tasks;
using ReserveFlow.Core.Services;
using ReserveFlow.Core.ViewModels;
using Xunit;

namespace ReserveFlow.Core.Tests;

public class RecordingViewModelTests
{
    [Fact]
    public async Task StartStopRecording_UpdatesProperties()
    {
        var audioSvc = new DummyAudioService();
        var storage = new DummyStorageService();
        var speech = new DummySpeechService();
        var vm = new RecordingViewModel(audioSvc, storage, speech);

        await vm.StartRecordingAsync();
        Assert.True(vm.IsRecording);

        await vm.StopRecordingAsync();
        Assert.False(vm.IsRecording);
        // Since dummy returns null file path, LastFilePath remains null
        Assert.Null(vm.LastFilePath);
    }

    private class DummyAudioService : IAudioService
    {
        public Task PlayAsync(string filePath) => Task.CompletedTask;
        public Task StartRecordingAsync() => Task.CompletedTask;
        public Task<(string? FilePath, System.TimeSpan Duration)> StopRecordingAsync()
            => Task.FromResult<(string?, System.TimeSpan)>((null, System.TimeSpan.Zero));
    }

    private class DummyStorageService : IStorageService
    {
        public Task InitializeAsync() => Task.CompletedTask;
        public Task SaveAttachmentAsync(ReserveFlow.Core.Models.GeotaggedAudioAttachment attachment) => Task.CompletedTask;
    }

    private class DummySpeechService : ISpeechService
    {
        public event System.Action<string>? PartialResult;
        public event System.Action<string>? FinalResult;

        public Task StartRecognitionAsync() => Task.CompletedTask;
        public Task StopRecognitionAsync() => Task.CompletedTask;
    }
}
