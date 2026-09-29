using CompressKly.Application.Videos.Compress;
using CompressKly.Application.Videos.Download;
using CompressKly.Application.Videos.GetStatus;
using CompressKly.Application.Videos.Upload;
using CompressKly.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using ValidationException = CompressKly.Domain.Exceptions.ValidationException;

namespace CompressKly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VideosController : ControllerBase
{
    private readonly UploadVideoHandler _uploadHandler;
    private readonly CompressVideoHandler _compressHandler;
    private readonly GetCompressionStatusHandler _statusHandler;
    private readonly DownloadVideoHandler _downloadHandler;

    public VideosController(
        UploadVideoHandler uploadHandler,
        CompressVideoHandler compressHandler,
        GetCompressionStatusHandler statusHandler,
        DownloadVideoHandler downloadHandler)
    {
        _uploadHandler = uploadHandler;
        _compressHandler = compressHandler;
        _statusHandler = statusHandler;
        _downloadHandler = downloadHandler;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(524_288_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
    public async Task<IActionResult> Upload(IFormFile video, CancellationToken cancellationToken)
    {
        if (video is null)
        {
            throw new ValidationException("O arquivo de vídeo é obrigatório.");
        }

        await using var stream = video.OpenReadStream();

        var result = await _uploadHandler.HandleAsync(
            new UploadVideoCommand
            {
                Content = stream,
                FileName = video.FileName,
                ContentType = video.ContentType,
                Size = video.Length
            },
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/compress")]
    public async Task<IActionResult> Compress(
        Guid id,
        [FromBody] CompressRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryParseQuality(request.Quality, out var quality))
        {
            throw new ValidationException("Qualidade de compressão inválida.");
        }

        var result = await _compressHandler.HandleAsync(
            new CompressVideoCommand
            {
                VideoId = id,
                Quality = quality
            },
            cancellationToken);

        return Accepted(result);
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _statusHandler.HandleAsync(
            new GetCompressionStatusQuery { VideoId = id },
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(
        Guid id,
        [FromQuery] bool preview = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _downloadHandler.HandleAsync(
            new DownloadVideoQuery
            {
                VideoId = id,
                AsAttachment = !preview,
                CleanupAfter = !preview
            },
            cancellationToken);

        if (preview)
        {
            return File(result.Content, result.ContentType, enableRangeProcessing: true);
        }

        return File(result.Content, result.ContentType, result.FileName);
    }

    private static bool TryParseQuality(string? value, out CompressionQuality quality)
    {
        quality = CompressionQuality.Balanced;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return Enum.TryParse(value, ignoreCase: true, out quality);
    }

    public sealed record CompressRequest(string Quality);
}
