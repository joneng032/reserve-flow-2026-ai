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

    public Task StartRecordingAsync()
    {
        // Create temp file
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recordings");
        Directory.CreateDirectory(dir);
        _currentFilePath = Path.Combine(dir, $"rec_{DateTime.UtcNow:yyyyMMdd_HHmmss}.wav");

        _waveIn = new WaveInEvent();
        _waveIn.WaveFormat = new WaveFormat(16000, 1);
        _writer = new WaveFileWriter(_currentFilePath, _waveIn.WaveFormat);
        _waveIn.DataAvailable += (s, a) => _writer?.Write(a.Buffer, 0, a.BytesRecorded);
        _waveIn.StartRecording();

        return Task.CompletedTask;
    }

    public Task<(string? FilePath, TimeSpan Duration)> StopRecordingAsync()
    {
        if (_waveIn == null || _writer == null || _currentFilePath == null)
            return Task.FromResult<(string?, TimeSpan)>((null, TimeSpan.Zero));

        _waveIn.StopRecording();
        _writer.Flush();
        _writer.Dispose();
        _waveIn.Dispose();

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

    public Task PlayAsync(string filePath)
    {
        try
        {
            using var audioFile = new AudioFileReader(filePath);
            using var output = new WaveOutEvent();
            output.Init(audioFile);
            output.Play();
            while (output.PlaybackState == PlaybackState.Playing)
            {
                Task.Delay(100).Wait();
            }
        }
        catch
        {
            // ignore
        }

        return Task.CompletedTask;
    }
}
