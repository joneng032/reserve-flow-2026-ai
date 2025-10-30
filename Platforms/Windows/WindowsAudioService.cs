using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using ReserveFlow.Core.Services;

namespace reserve_flow_ai_2026.Platforms.Windows;

public class WindowsAudioService : IAudioService
{
    private WaveInEvent? _waveIn;
    private WaveFileWriter? _writer;
    private string? _currentFilePath;
    private readonly object _lock = new();

    public Task StartRecordingAsync()
    {
        lock (_lock)
        {
            if (_waveIn != null)
                return Task.CompletedTask; // already recording

            try
            {
                // Create temp file
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recordings");
                Directory.CreateDirectory(dir);
                _currentFilePath = Path.Combine(dir, $"rec_{DateTime.UtcNow:yyyyMMdd_HHmmss}.wav");

                _waveIn = new WaveInEvent();
                _waveIn.WaveFormat = new WaveFormat(16000, 1);
                _writer = new WaveFileWriter(_currentFilePath, _waveIn.WaveFormat);
                _waveIn.DataAvailable += OnDataAvailable;
                _waveIn.StartRecording();
            }
            catch
            {
                _writer?.Dispose();
                _writer = null;
                _waveIn?.Dispose();
                _waveIn = null;
                _currentFilePath = null;
            }
        }

        return Task.CompletedTask;
    }

    public Task<(string? FilePath, TimeSpan Duration)> StopRecordingAsync()
    {
        lock (_lock)
        {
            if (_waveIn == null || _writer == null || _currentFilePath == null)
                return Task.FromResult<(string?, TimeSpan)>((null, TimeSpan.Zero));

            try
            {
                _waveIn.StopRecording();
            }
            catch { }

            try
            {
                _writer.Flush();
                _writer.Dispose();
            }
            catch { }

            try
            {
                _waveIn.DataAvailable -= OnDataAvailable;
                _waveIn.Dispose();
            }
            catch { }

            // approximate duration
            var duration = TimeSpan.Zero;
            try
            {
                using var reader = new WaveFileReader(_currentFilePath);
                duration = reader.TotalTime;
            }
            catch { }

            var path = _currentFilePath;

            _writer = null;
            _waveIn = null;
            _currentFilePath = null;

            return Task.FromResult<(string?, TimeSpan)>((path, duration));
        }
    }

    public Task PlayAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.CompletedTask;

        // Play asynchronously without blocking the caller thread
        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            var audioFile = new AudioFileReader(filePath);
            var output = new WaveOutEvent();
            output.Init(audioFile);

            output.PlaybackStopped += (s, e) =>
            {
                try { output.Dispose(); } catch { }
                try { audioFile.Dispose(); } catch { }
                tcs.TrySetResult(null);
            };

            output.Play();
        }
        catch
        {
            tcs.TrySetResult(null);
        }

        return tcs.Task;
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        try
        {
            _writer?.Write(e.Buffer, 0, e.BytesRecorded);
        }
        catch { }
    }
}
