using CompressKly.Application.Videos.Upload;
using FluentAssertions;

namespace CompressKly.UnitTests;

public class UploadVideoValidatorTests
{
    [Fact]
    public void Validate_ValidMp4_ShouldPass()
    {
        var validator = new UploadVideoValidator();
        var command = new UploadVideoCommand
        {
            Content = Stream.Null,
            FileName = "video.mp4",
            ContentType = "video/mp4",
            Size = 1024
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NonMp4_ShouldFail()
    {
        var validator = new UploadVideoValidator();
        var command = new UploadVideoCommand
        {
            Content = Stream.Null,
            FileName = "video.avi",
            ContentType = "video/avi",
            Size = 1024
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
