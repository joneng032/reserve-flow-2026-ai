using System;
using System.IO;
using System.Threading.Tasks;
using Android.Media;
// avoid ambiguous Application symbol (Microsoft.Maui.Controls.Application vs Android.App.Application)
using ReserveFlow.Core.Services;

namespace reserve_flow_ai_2026.Platforms.Android;

public class AndroidAudioService : IAudioService
{
    private MediaRecorder? _recorder;
    private string? _currentFilePath;
    private DateTime _startTime;

    public Task StartRecordingAsync()
    {
        try
        {
            var filesDir = global::Android.App.Application.Context.FilesDir?.AbsolutePath ?? ".";
            var dir = Path.Combine(filesDir, "recordings");
            Directory.CreateDirectory(dir);

            var fileName = $"rec_{DateTime.UtcNow:yyyyMMdd_HHmmss}.m4a";
            _currentFilePath = Path.Combine(dir, fileName);

            _recorder = new MediaRecorder();
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _recorder.SetAudioSamplingRate(16000);
            _recorder.SetOutputFile(_currentFilePath);
            _recorder.Prepare();
            _recorder.Start();

            _startTime = DateTime.UtcNow;
        }
        catch
        {
            // swallow for scaffold; real implementation should surface errors
            _currentFilePath = null;
            _recorder = null;
        }

        return Task.CompletedTask;
    }

    public Task<(string? FilePath, TimeSpan Duration)> StopRecordingAsync()
    {
        try
        {
            if (_recorder == null || _currentFilePath == null)
                return Task.FromResult<(string?, TimeSpan)>((null, TimeSpan.Zero));

            try
            {
                _recorder.Stop();
            }
            catch { /* ignore stop exceptions on some devices */ }

            _recorder.Release();
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
            var player = new MediaPlayer();
            player.SetDataSource(filePath);
            player.Prepare();
            player.Start();

            // Detach after playback completes; run on background Task
            Task.Run(() =>
            {
                try
                {
                    while (player.IsPlaying)
                    {
                        Task.Delay(200).Wait();
                    }
                }
                catch { }
                finally
                {
                    try { player.Stop(); } catch { }
                    player.Release();
                    player.Dispose();
                }
            });
        }
        catch
        {
            // ignore in scaffold
        }

        return Task.CompletedTask;
    }
}
