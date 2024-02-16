using System.Text.Json.Serialization;

namespace TED.API.DTOs;

/// <summary>
/// DTO para requisições administrativas relacionadas a TEDs.
/// </summary>
public class AdminTedRequestDto
{
    /// <summary>
    /// Motivo da reprovação de um TED, se aplicável.
    /// </summary>
    [JsonPropertyName("motiveRejection")]
    public string? MotivoReprovacao { get; set; }
}
