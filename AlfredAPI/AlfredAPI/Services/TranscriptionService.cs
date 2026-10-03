using System.Text;
using Whisper.net;

namespace AlfredAPI.Services;

public class TranscriptionService : ITranscriptionService
{
    private readonly string _modelPath;

    public TranscriptionService(IWebHostEnvironment environment)
    {
        _modelPath = Path.Combine(
            environment.ContentRootPath,
            "Models",
            "Whisper",
            "ggml-small.bin");
    }

    public async Task<string> TranscribeAsync(
        IFormFile audio,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_modelPath))
        {
            throw new FileNotFoundException(
                $"Whisper model not found at '{_modelPath}'. " +
                "Place ggml-small.bin in Models/Whisper before running transcription.");
        }

        using var whisperFactory = WhisperFactory.FromPath(_modelPath);
        using var processor = whisperFactory
            .CreateBuilder()
            .WithLanguage("pt")
            .Build();

        await using var audioStream = audio.OpenReadStream();
        var transcription = new StringBuilder();

        await foreach (var segment in processor.ProcessAsync(audioStream))
        {
            cancellationToken.ThrowIfCancellationRequested();
            transcription.Append(segment.Text);
        }

        return transcription.ToString().Trim();
    }
}
