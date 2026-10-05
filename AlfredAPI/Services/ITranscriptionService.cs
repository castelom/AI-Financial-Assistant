namespace AlfredAPI.Services;

public interface ITranscriptionService
{
    Task<string> TranscribeAsync(
        Stream audioStream,
        CancellationToken cancellationToken = default);
}