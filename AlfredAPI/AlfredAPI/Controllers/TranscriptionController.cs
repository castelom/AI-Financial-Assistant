using AlfredAPI.Models;
using AlfredAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlfredAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranscriptionController : ControllerBase
{
    private readonly ITranscriptionService _transcriptionService;

    public TranscriptionController(ITranscriptionService transcriptionService)
    {
        _transcriptionService = transcriptionService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(TranscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transcribe(
        IFormFile? audio,
        CancellationToken cancellationToken)
    {
        if (audio is null || audio.Length == 0)
            return BadRequest(new { message = "An audio file is required." });

        var text = await _transcriptionService.TranscribeAsync(audio, cancellationToken);

        return Ok(new TranscriptionResponse { Text = text });
    }
}
