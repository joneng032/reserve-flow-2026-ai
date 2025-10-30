using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ReserveFlow.Core.Models;
using ReserveFlow.Core.Utilities;
using ReserveFlow.Core.Services;

namespace ReserveFlow.Core.ViewModels;

public class RecordingViewModel : INotifyPropertyChanged
{
    private readonly IAudioService _audioService;
    private readonly IStorageService _storageService;
    private readonly ISpeechService _speechService;

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isRecording;
    public bool IsRecording
    {
        get => _isRecording;
        set { _isRecording = value; OnPropertyChanged(); }
    }

    private string? _lastFilePath;
    public string? LastFilePath
    {
        get => _lastFilePath;
        set { _lastFilePath = value; OnPropertyChanged(); }
    }

    private string _transcription = string.Empty;
    public string Transcription
    {
        get => _transcription;
        set { _transcription = value; OnPropertyChanged(); }
    }

    public ICommand StartRecordingCommand { get; }
    public ICommand StopRecordingCommand { get; }
    public ICommand PlayCommand { get; }

    public RecordingViewModel(IAudioService audioService, IStorageService storageService, ISpeechService speechService)
    {
        _audioService = audioService;
        _storageService = storageService;
        _speechService = speechService;

        // Subscribe to speech events (partial and final results)
        _speechService.PartialResult += (txt) =>
        {
            Transcription = txt;
        };

        _speechService.FinalResult += (txt) =>
        {
            Transcription = string.IsNullOrEmpty(Transcription) ? txt : Transcription + " " + txt;
        };

        StartRecordingCommand = new RelayCommand(async () => await StartRecordingAsync());
        StopRecordingCommand = new RelayCommand(async () => await StopRecordingAsync());
        PlayCommand = new RelayCommand(async (p) => await PlayAsync(p as string));
    }

    public async Task StartRecordingAsync()
    {
        await _audioService.StartRecordingAsync();
        IsRecording = true;
    }

    public async Task StopRecordingAsync()
    {
        var result = await _audioService.StopRecordingAsync();
        IsRecording = false;
        LastFilePath = result.FilePath;
        // In a real implementation, we would kick off transcription here.
        if (result.FilePath != null)
        {
            Transcription = string.Empty; // placeholder
            // Persist to local storage
            var attach = new GeotaggedAudioAttachment
            {
                FilePath = result.FilePath,
                Duration = result.Duration,
                Timestamp = System.DateTimeOffset.UtcNow
            };

            await _storageService.SaveAttachmentAsync(attach);
        }
    }

    public async Task PlayAsync(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        await _audioService.PlayAsync(filePath);
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
