using ReserveFlow.Core.ViewModels;

namespace reserve_flow_ai_2026;

public partial class RecordingPage : ContentPage
{
    private readonly RecordingViewModel _vm;

    public RecordingPage(RecordingViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = _vm;

        StartBtn.Clicked += async (_, __) => await _vm.StartRecordingAsync();
        StopBtn.Clicked += async (_, __) => await _vm.StopRecordingAsync();
        PlayBtn.Clicked += async (_, __) => await _vm.PlayAsync(_vm.LastFilePath);

        // Wire transcription editor to ViewModel property
        TranscriptionEditor.TextChanged += (s, e) => _vm.Transcription = TranscriptionEditor.Text;
        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_vm.LastFilePath))
            {
                // update UI if needed
            }
            if (e.PropertyName == nameof(_vm.Transcription))
            {
                if (TranscriptionEditor.Text != _vm.Transcription)
                    TranscriptionEditor.Text = _vm.Transcription;
            }
        };

        SaveBtn.Clicked += async (_, __) =>
        {
            // For scaffold: just show a confirmation dialog
            await DisplayAlert("Save", "Recording saved (scaffold)", "OK");
        };
    }
}
