using AlfredAPI.Models;
using AlfredAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlfredAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranscriptionController : ControllerBase
{
    private readonly ITranscriptionService _transcriptionService;
    private readonly IAudioConversionService _audioConversionService;

    public TranscriptionController(
        ITranscriptionService transcriptionService,
        IAudioConversionService audioConversionService)
    {
        _transcriptionService = transcriptionService;
        _audioConversionService = audioConversionService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(
        typeof(TranscriptionResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transcribe(
        IFormFile? audio,
        CancellationToken cancellationToken)
    {
        if (audio is null || audio.Length == 0)
        {
            return BadRequest(
                new { message = "An audio file is required." });
        }

        string? wavPath = null;

        try
        {
            wavPath =
                await _audioConversionService.ConvertToWavAsync(
                    audio,
                    cancellationToken);

            await using var wavStream =
                System.IO.File.OpenRead(wavPath);

            var text =
                await _transcriptionService.TranscribeAsync(
                    wavStream,
                    cancellationToken);

            return Ok(
                new TranscriptionResponse
                {
                    Text = text
                });
        }
        finally
        {
            if (wavPath is not null &&
                System.IO.File.Exists(wavPath))
            {
                System.IO.File.Delete(wavPath);
            }
        }
    }
}