using CompressKly.Domain.Entities;
using CompressKly.Domain.Enums;
using CompressKly.Domain.Exceptions;
using FluentAssertions;

namespace CompressKly.UnitTests;

public class VideoTests
{
    [Fact]
    public void Create_ShouldInitializeUploadedVideo()
    {
        var video = Video.Create("demo.mp4", "demo.mp4", 1_000_000);

        video.Id.Should().NotBeEmpty();
        video.Status.Should().Be(CompressionStatus.Uploaded);
        video.Progress.Should().Be(0);
        video.CompressedFileName.Should().Be("demo-comprimido.mp4");
    }

    [Fact]
    public void QueueCompression_ShouldSetQualityAndQueuedStatus()
    {
        var video = Video.Create("demo.mp4", "demo.mp4", 1_000_000);

        video.QueueCompression(CompressionQuality.Max);

        video.Quality.Should().Be(CompressionQuality.Max);
        video.Status.Should().Be(CompressionStatus.Queued);
        video.Progress.Should().Be(5);
    }

    [Fact]
    public void MarkCompleted_ShouldCalculateSavings()
    {
        var video = Video.Create("demo.mp4", "demo.mp4", 1_000_000);
        video.QueueCompression(CompressionQuality.Balanced);

        video.MarkCompleted(400_000);

        video.Status.Should().Be(CompressionStatus.Completed);
        video.CompressedSize.Should().Be(400_000);
        video.SavedBytes.Should().Be(600_000);
        video.ReductionPercent.Should().BeApproximately(60d, 0.01);
    }

    [Fact]
    public void Create_WithInvalidSize_ShouldThrow()
    {
        var act = () => Video.Create("demo.mp4", "demo.mp4", 0);

        act.Should().Throw<ValidationException>();
    }
}
