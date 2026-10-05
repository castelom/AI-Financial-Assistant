namespace AlfredAPI.Services;

public interface IAudioConversionService
{
    Task<string> ConvertToWavAsync(
        IFormFile audio,
        CancellationToken cancellationToken = default);
}