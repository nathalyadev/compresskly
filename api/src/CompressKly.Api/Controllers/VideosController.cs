using CompressKly.Api.Contracts;
using CompressKly.Application.DTOs;
using CompressKly.Application.Videos.Compress;
using CompressKly.Application.Videos.Download;
using CompressKly.Application.Videos.GetStatus;
using CompressKly.Application.Videos.Upload;
using CompressKly.Core.Enums;
using CompressKly.Core.Interfaces.Notifications;
using Microsoft.AspNetCore.Mvc;
using ValidationException = CompressKly.Core.Exceptions.ValidationException;

namespace CompressKly.Api.Controllers
{

    /// <summary>
    /// Upload, compressão, acompanhamento e download de vídeos MP4.
    /// </summary>
    public class VideosController(
        UploadVideoHandler uploadHandler,
        CompressVideoHandler compressHandler,
        GetCompressionStatusHandler statusHandler,
        DownloadVideoHandler downloadHandler,
        INotifier notifier) : MainController(notifier)
    {

        /// <summary>
        /// Envia um vídeo MP4 para o servidor.
        /// </summary>
        /// <remarks>
        /// O limite de tamanho segue a configuração `Upload:MaxFileSizeMb` (padrão 500 MB).
        /// </remarks>
        /// <param name="video">Arquivo de vídeo (multipart/form-data).</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <response code="200">Upload aceito; retorna o identificador do vídeo.</response>
        /// <response code="400">Arquivo ausente ou inválido.</response>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UploadVideoResponse), StatusCodes.Status200OK)]
        [RequestSizeLimit(524_288_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        public async Task<IActionResult> Upload(IFormFile video, CancellationToken cancellationToken)
        {
            if (video is null)
            {
                throw new ValidationException("O arquivo de vídeo é obrigatório.");
            }

            await using var stream = video.OpenReadStream();

            var result = await uploadHandler.HandleAsync(
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

        /// <summary>
        /// Inicia a compressão assíncrona de um vídeo.
        /// </summary>
        /// <param name="id">Identificador retornado no upload.</param>
        /// <param name="request">Qualidade desejada.</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <response code="202">Job de compressão aceito.</response>
        /// <response code="400">Qualidade inválida.</response>
        /// <response code="404">Vídeo não encontrado.</response>
        /// <response code="422">Falha ao processar o vídeo.</response>
        [HttpPost("{id:guid}/compress")]
        [ProducesResponseType(typeof(CompressVideoResponse), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Compress(
            Guid id,
            [FromBody] CompressVideoRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryParseQuality(request.Quality, out var quality))
            {
                throw new ValidationException("Qualidade de compressão inválida.");
            }

            var result = await compressHandler.HandleAsync(
                new CompressVideoCommand
                {
                    VideoId = id,
                    Quality = quality
                },
                cancellationToken);

            return Accepted(result);
        }

        /// <summary>
        /// Consulta status e progresso da compressão.
        /// </summary>
        /// <param name="id">Identificador do vídeo.</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <response code="200">Estado atual do processamento.</response>
        /// <response code="404">Vídeo não encontrado.</response>
        [HttpGet("{id:guid}/status")]
        [ProducesResponseType(typeof(CompressionStatusResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatus(Guid id, CancellationToken cancellationToken)
        {
            var result = await statusHandler.HandleAsync(
                new GetCompressionStatusQuery { VideoId = id },
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Baixa ou reproduz o vídeo comprimido.
        /// </summary>
        /// <param name="id">Identificador do vídeo.</param>
        /// <param name="preview">
        /// Se <c>true</c>, retorna o stream para preview (sem forçar download e sem limpar o arquivo após a resposta).
        /// Se <c>false</c> (padrão), envia como anexo para download.
        /// </param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <response code="200">Conteúdo binário do vídeo (tipo conforme arquivo).</response>
        /// <response code="404">Vídeo ou arquivo comprimido não encontrado.</response>
        [HttpGet("{id:guid}/download")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Download(
            Guid id,
            [FromQuery] bool preview = false,
            CancellationToken cancellationToken = default)
        {
            var result = await downloadHandler.HandleAsync(
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
    }
}