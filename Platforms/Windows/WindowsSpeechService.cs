using System;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using ReserveFlow.Core.Services;

namespace reserve_flow_ai_2026.Platforms.Windows;

public class WindowsSpeechService : ISpeechService
{
    private SpeechRecognizer? _recognizer;

    public event Action<string>? PartialResult;
    public event Action<string>? FinalResult;

    public async Task StartRecognitionAsync()
    {
        _recognizer = new SpeechRecognizer();

        _recognizer.HypothesisGenerated += Recognizer_HypothesisGenerated;

        _recognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

        await _recognizer.CompileConstraintsAsync();
        await _recognizer.ContinuousRecognitionSession.StartAsync();
    }

    private void Recognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
    {
        try
        {
            PartialResult?.Invoke(args.Hypothesis.Text);
        }
        catch { }
    }

    private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        try
        {
            if (args.Result.Status == SpeechRecognitionResultStatus.Success)
            {
                FinalResult?.Invoke(args.Result.Text);
            }
        }
        catch { }
    }

    public async Task StopRecognitionAsync()
    {
        if (_recognizer == null)
            return;

        try
        {
            await _recognizer.ContinuousRecognitionSession.StopAsync();
        }
        catch { }

        try
        {
            _recognizer.HypothesisGenerated -= Recognizer_HypothesisGenerated;
            _recognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
            _recognizer.Dispose();
        }
        catch { }

        _recognizer = null;
    }
}
