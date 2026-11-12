using System.Diagnostics;
using System.Globalization;
using System.Text;
using CompressKly.Application.Interfaces;
using CompressKly.Core.Enums;
using CompressKly.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CompressKly.Infrastructure.Compression;

public sealed class FfmpegOptions
{
    public const string SectionName = "Ffmpeg";

    public string FfmpegPath { get; set; } = "ffmpeg";
    public string FfprobePath { get; set; } = "ffprobe";
}

public sealed class FfmpegVideoCompressor : IVideoCompressor
{
    private readonly FfmpegOptions _options;
    private readonly ILogger<FfmpegVideoCompressor> _logger;

    public FfmpegVideoCompressor(IOptions<FfmpegOptions> options, ILogger<FfmpegVideoCompressor> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task CompressAsync(
        string inputPath,
        string outputPath,
        CompressionQuality quality,
        Action<int>? onProgress = null,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        var preset = GetPreset(quality);
        var durationSeconds = await ProbeDurationAsync(inputPath, cancellationToken);

        var args = new StringBuilder();
        args.Append("-y ");
        args.Append(CultureInfo.InvariantCulture, $"-i \"{inputPath}\" ");
        args.Append("-c:v libx264 ");
        args.Append("-c:a aac ");
        args.Append(CultureInfo.InvariantCulture, $"-b:a {preset.AudioBitrate} ");
        args.Append(CultureInfo.InvariantCulture, $"-crf {preset.Crf} ");
        args.Append(CultureInfo.InvariantCulture, $"-preset {preset.Preset} ");
        args.Append("-movflags +faststart ");
        args.Append("-pix_fmt yuv420p ");

        if (!string.IsNullOrWhiteSpace(preset.Scale))
        {
            args.Append(CultureInfo.InvariantCulture, $"-vf \"{preset.Scale}\" ");
        }

        args.Append($"\"{outputPath}\"");

        var startInfo = new ProcessStartInfo
        {
            FileName = _options.FfmpegPath,
            Arguments = args.ToString(),
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

        try
        {
            if (!process.Start())
            {
                throw new CompressionException("O processador de vídeo está indisponível no momento. Tente mais tarde.");
            }
        }
        catch (Exception ex) when (ex is not CompressionException)
        {
            throw new CompressionException(
                "O processador de vídeo está indisponível no momento. Tente mais tarde.",
                ex);
        }

        var stderr = new StringBuilder();
        process.ErrorDataReceived += (_, e) =>
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            stderr.AppendLine(e.Data);
            var percent = TryParseProgress(e.Data, durationSeconds);
            if (percent is not null)
            {
                onProgress?.Invoke(percent.Value);
            }
        };

        process.BeginErrorReadLine();
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            _logger.LogError("FFmpeg falhou (code {Code}): {Error}", process.ExitCode, stderr.ToString());
            throw new CompressionException("Não foi possível comprimir o vídeo. Tente novamente.");
        }

        onProgress?.Invoke(100);
    }

    private async Task<double?> ProbeDurationAsync(string inputPath, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _options.FfprobePath,
            Arguments =
                $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            if (double.TryParse(output.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var duration))
            {
                return duration;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível obter duração do vídeo via ffprobe.");
        }

        return null;
    }

    private static int? TryParseProgress(string line, double? durationSeconds)
    {
        // time=00:00:12.34
        const string marker = "time=";
        var index = line.IndexOf(marker, StringComparison.Ordinal);
        if (index < 0 || durationSeconds is null or <= 0)
        {
            return null;
        }

        var token = line[(index + marker.Length)..].Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (token is null || !TimeSpan.TryParse(token, CultureInfo.InvariantCulture, out var elapsed))
        {
            return null;
        }

        var percent = (int)Math.Round(elapsed.TotalSeconds / durationSeconds.Value * 100d);
        return Math.Clamp(percent, 0, 99);
    }

    private static QualityPreset GetPreset(CompressionQuality quality) => quality switch
    {
        CompressionQuality.High => new QualityPreset(20, "medium", "160k", null),
        CompressionQuality.Max => new QualityPreset(28, "faster", "96k", "scale='min(1280,iw)':-2"),
        _ => new QualityPreset(23, "medium", "128k", null)
    };

    private sealed record QualityPreset(int Crf, string Preset, string AudioBitrate, string? Scale);
}
