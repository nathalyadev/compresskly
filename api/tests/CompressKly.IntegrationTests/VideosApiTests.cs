using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace CompressKly.IntegrationTests;

public class VideosApiTests : IClassFixture<CompressKlyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VideosApiTests(CompressKlyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Upload_WithoutFile_ShouldReturnBadRequest()
    {
        using var content = new MultipartFormDataContent();
        var response = await _client.PostAsync("/api/videos/upload", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_ValidMp4_ShouldReturnOk()
    {
        var bytes = Encoding.UTF8.GetBytes("fake-mp4-content-for-upload-test");
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
        content.Add(fileContent, "video", "sample.mp4");

        var response = await _client.PostAsync("/api/videos/upload", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("id").GetGuid().Should().NotBeEmpty();
        document.RootElement.GetProperty("originalFileName").GetString().Should().Be("sample.mp4");
    }

    [Fact]
    public async Task GetStatus_UnknownId_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/videos/{Guid.NewGuid()}/status");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
