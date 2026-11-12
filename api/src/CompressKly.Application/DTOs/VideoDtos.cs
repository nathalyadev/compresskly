namespace CompressKly.Application.DTOs;

/// <summary>
/// Resultado do upload de um vídeo.
/// </summary>
/// <param name="Id">Identificador único do vídeo.</param>
/// <param name="OriginalFileName">Nome do arquivo enviado.</param>
/// <param name="OriginalSize">Tamanho em bytes do arquivo original.</param>
/// <param name="Status">Estado inicial do vídeo na API.</param>
public sealed record UploadVideoResponse(
    Guid Id,
    string OriginalFileName,
    long OriginalSize,
    string Status);

/// <summary>
/// Confirmação de que o job de compressão foi enfileirado ou iniciado.
/// </summary>
/// <param name="Id">Identificador do vídeo.</param>
/// <param name="Status">Estado após solicitar compressão.</param>
/// <param name="Progress">Progresso inicial (0–100).</param>
public sealed record CompressVideoResponse(
    Guid Id,
    string Status,
    int Progress);

/// <summary>
/// Status detalhado da compressão, incluindo métricas quando disponíveis.
/// </summary>
/// <param name="Id">Identificador do vídeo.</param>
/// <param name="Status">Estado do processamento.</param>
/// <param name="Progress">Progresso estimado (0–100).</param>
/// <param name="Message">Mensagem amigável de progresso.</param>
/// <param name="OriginalFileName">Nome do arquivo original.</param>
/// <param name="OriginalSize">Tamanho original em bytes.</param>
/// <param name="CompressedSize">Tamanho após compressão, se concluído.</param>
/// <param name="SavedBytes">Bytes economizados.</param>
/// <param name="ReductionPercent">Percentual de redução.</param>
/// <param name="DownloadUrl">URL relativa sugerida para download.</param>
/// <param name="FileName">Nome sugerido do arquivo comprimido.</param>
/// <param name="ErrorMessage">Detalhe em caso de falha.</param>
public sealed record CompressionStatusResponse(
    Guid Id,
    string Status,
    int Progress,
    string? Message,
    string OriginalFileName,
    long OriginalSize,
    long? CompressedSize,
    long? SavedBytes,
    double? ReductionPercent,
    string? DownloadUrl,
    string? FileName,
    string? ErrorMessage);

/// <summary>
/// Dados do stream para entrega do arquivo comprimido (uso interno na aplicação).
/// </summary>
public sealed record DownloadVideoResult(
    Stream Content,
    string FileName,
    string ContentType,
    long ContentLength);
