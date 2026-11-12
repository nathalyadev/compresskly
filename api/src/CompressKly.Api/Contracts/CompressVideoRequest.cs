using System.ComponentModel;

namespace CompressKly.Api.Contracts;

/// <summary>
/// Parâmetros para iniciar a compressão de um vídeo já enviado.
/// </summary>
public sealed class CompressVideoRequest
{
    /// <summary>
    /// Preset de qualidade: <c>high</c>, <c>balanced</c> ou <c>max</c>.
    /// </summary>
    /// <example>balanced</example>
    [DefaultValue("balanced")]
    public string Quality { get; set; } = "balanced";
}
