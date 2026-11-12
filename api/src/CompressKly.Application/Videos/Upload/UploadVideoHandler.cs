using CompressKly.Application.DTOs;
using CompressKly.Application.Interfaces;
using CompressKly.Core.Entities;
using FluentValidation;
using CoreValidationException = CompressKly.Core.Exceptions.ValidationException;

namespace CompressKly.Application.Videos.Upload;

public sealed class UploadVideoHandler
{
    private readonly IVideoRepository _repository;
    private readonly IVideoStorage _storage;
    private readonly IValidator<UploadVideoCommand> _validator;

    public UploadVideoHandler(
        IVideoRepository repository,
        IVideoStorage storage,
        IValidator<UploadVideoCommand> validator)
    {
        _repository = repository;
        _storage = storage;
        _validator = validator;
    }

    public async Task<UploadVideoResponse> HandleAsync(
        UploadVideoCommand command,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new CoreValidationException(
                validation.Errors.First().ErrorMessage,
                errors);
        }

        var safeFileName = SanitizeFileName(command.FileName);
        var video = Video.Create(command.FileName, safeFileName, command.Size);

        await _storage.SaveOriginalAsync(video.Id, safeFileName, command.Content, cancellationToken);
        await _repository.AddAsync(video, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new UploadVideoResponse(
            video.Id,
            video.OriginalFileName,
            video.OriginalSize,
            video.Status.ToString());
    }

    private static string SanitizeFileName(string originalName)
    {
        var baseName = Path.GetFileName(originalName);
        var sanitized = new string(baseName
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '.' or '_' or '-' ? ch : '_')
            .ToArray());

        while (sanitized.Contains("__", StringComparison.Ordinal))
        {
            sanitized = sanitized.Replace("__", "_", StringComparison.Ordinal);
        }

        if (sanitized.Length > 120)
        {
            sanitized = sanitized[..120];
        }

        var withoutExt = sanitized.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
            ? sanitized[..^4]
            : sanitized;

        var safe = string.IsNullOrWhiteSpace(withoutExt) ? "video" : withoutExt;
        return $"{safe}.mp4";
    }
}
