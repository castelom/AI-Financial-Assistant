using System.Text;
using Whisper.net;

namespace AlfredAPI.Services;

public class TranscriptionService : ITranscriptionService, IDisposable
{
    private readonly WhisperFactory _whisperFactory;
    private readonly string _language;

    public TranscriptionService(
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var relativeModelPath =
            configuration["Whisper:ModelPath"]
            ?? throw new InvalidOperationException(
                "Whisper model path is not configured.");

        _language =
            configuration["Whisper:Language"]
            ?? "pt";

        var modelPath = Path.Combine(
            environment.ContentRootPath,
            relativeModelPath);

        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException(
                $"Whisper model not found at: {modelPath}");
        }

        _whisperFactory = WhisperFactory.FromPath(modelPath);
    }

    public async Task<string> TranscribeAsync(
    Stream audioStream,
    CancellationToken cancellationToken = default)
    {
        if (audioStream is null)
        {
            throw new ArgumentNullException(nameof(audioStream));
        }

        using var processor = _whisperFactory
            .CreateBuilder()
            .WithLanguage(_language)
            .Build();

        var transcription = new StringBuilder();

        await foreach (var segment in processor.ProcessAsync(
            audioStream,
            cancellationToken))
        {
            transcription.Append(segment.Text);
        }

        return transcription.ToString().Trim();
    }

    public void Dispose()
    {
        _whisperFactory.Dispose();
    }
}