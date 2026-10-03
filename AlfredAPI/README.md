# AlfredAPI — local Whisper transcription

ASP.NET Core Web API for the Alfred AI Financial Assistant.

Current flow:

`audio -> POST /api/transcription -> Whisper.net (local CPU) -> text`

No OpenAI API key is required.

## Requirements

- .NET 8 SDK
- A CPU compatible with the default Whisper.net runtime
- `ggml-small.bin` placed at `Models/Whisper/ggml-small.bin`

## Packages

- Whisper.net 1.9.1
- Whisper.net.Runtime 1.9.1 (CPU)

## Run

```bash
dotnet restore
dotnet run
```

Default HTTP URL: `http://localhost:5080`

## Endpoint

`POST /api/transcription`

Send `multipart/form-data` using the field name `audio`.

Example response:

```json
{
  "text": "Recebi 850 reais da Maria pelo pedido"
}
```

## Important: browser audio

This first backend implementation expects audio in a format Whisper.net can process directly.
When connecting Angular MediaRecorder, we will test the browser output format and add a conversion step only if needed.
