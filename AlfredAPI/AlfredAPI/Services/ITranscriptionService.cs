namespace AlfredAPI.Services;

public interface ITranscriptionService
{
    Task<string> TranscribeAsync(IFormFile audio, CancellationToken cancellationToken = default);
}
