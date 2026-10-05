using System.Diagnostics;

namespace AlfredAPI.Services;

public class AudioConversionService : IAudioConversionService
{
    public async Task<string> ConvertToWavAsync(
        IFormFile audio,
        CancellationToken cancellationToken = default)
    {
        if (audio is null || audio.Length == 0)
        {
            throw new ArgumentException(
                "Audio file cannot be empty.",
                nameof(audio));
        }

        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "alfred-audio");

        Directory.CreateDirectory(tempDirectory);

        var id = Guid.NewGuid().ToString("N");

        var extension = Path.GetExtension(audio.FileName);

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".webm";
        }

        var inputPath = Path.Combine(
            tempDirectory,
            $"{id}{extension}");

        var outputPath = Path.Combine(
            tempDirectory,
            $"{id}.wav");

        try
        {
            await using (var fileStream = File.Create(inputPath))
            {
                await audio.CopyToAsync(
                    fileStream,
                    cancellationToken);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("-y");

            startInfo.ArgumentList.Add("-i");
            startInfo.ArgumentList.Add(inputPath);

            startInfo.ArgumentList.Add("-ar");
            startInfo.ArgumentList.Add("16000");

            startInfo.ArgumentList.Add("-ac");
            startInfo.ArgumentList.Add("1");

            startInfo.ArgumentList.Add("-c:a");
            startInfo.ArgumentList.Add("pcm_s16le");

            startInfo.ArgumentList.Add(outputPath);

            using var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            var errorOutput =
                process.StandardError.ReadToEndAsync(
                    cancellationToken);

            await process.WaitForExitAsync(
                cancellationToken);

            var ffmpegError = await errorOutput;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"FFmpeg conversion failed: {ffmpegError}");
            }

            if (!File.Exists(outputPath))
            {
                throw new InvalidOperationException(
                    "FFmpeg did not generate the WAV file.");
            }

            return outputPath;
        }
        finally
        {
            if (File.Exists(inputPath))
            {
                File.Delete(inputPath);
            }
        }
    }
}