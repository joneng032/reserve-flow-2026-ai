using System;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using ReserveFlow.Core.Services;

namespace reserve_flow_ai_2026.Platforms.iOS;

#if IOS || MACCATALYST
public class iOSAudioService : IAudioService
{
    private AVAudioRecorder? _recorder;
    private AVAudioPlayer? _player;
    private string? _currentFilePath;
    private DateTime _startTime;

    public Task StartRecordingAsync()
    {
        try
        {
            var session = AVAudioSession.SharedInstance();
            // Use synchronous APIs available on AVAudioSession in Xamarin.iOS/.NET
            session.SetCategory(AVAudioSession.CategoryRecord, out NSError catErr);
            session.SetActive(true, out NSError actErr);

            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "recordings");
            Directory.CreateDirectory(dir);
            _currentFilePath = Path.Combine(dir, $"rec_{DateTime.UtcNow:yyyyMMdd_HHmmss}.m4a");

            var settings = new AudioSettings
            {
                SampleRate = 16000.0f,
                NumberChannels = 1,
                Format = AudioToolbox.AudioFormatType.MPEG4AAC
            };

            var url = NSUrl.FromFilename(_currentFilePath);
            var recorder = AVAudioRecorder.Create(url, settings, out NSError err);
            if (recorder == null)
            {
                _recorder = null;
                _currentFilePath = null;
                return Task.CompletedTask;
            }

            _recorder = recorder;
            _recorder.PrepareToRecord();
            _recorder.Record();

            _startTime = DateTime.UtcNow;
        }
        catch
        {
            _recorder = null;
            _currentFilePath = null;
        }

        return Task.CompletedTask;
    }

    public Task<(string? FilePath, TimeSpan Duration)> StopRecordingAsync()
    {
        try
        {
            if (_recorder == null || _currentFilePath == null)
                return Task.FromResult<(string?, TimeSpan)>((null, TimeSpan.Zero));

            _recorder.Stop();
            _recorder.Dispose();
            _recorder = null;

            var duration = DateTime.UtcNow - _startTime;
            var path = _currentFilePath;
            _currentFilePath = null;

            return Task.FromResult<(string?, TimeSpan)>((path, duration));
        }
        catch
        {
            return Task.FromResult<(string?, TimeSpan)>((null, TimeSpan.Zero));
        }
    }

    public Task PlayAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.CompletedTask;

        try
        {
            var url = NSUrl.FromFilename(filePath);
            var player = AVAudioPlayer.FromUrl(url, out NSError err);
            if (player == null)
                return Task.CompletedTask;

            _player = player;
            _player.Play();
        }
        catch
        {
            // ignore in scaffold
        }

        return Task.CompletedTask;
    }
}
#endif
