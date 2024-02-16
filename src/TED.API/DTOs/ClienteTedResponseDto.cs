using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TED.API.Enums;

namespace TED.API.DTOs;

public class ClienteTedResponseDto: ClienteTedRequestDto
{
    /// <summary>
    /// Id do TED.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Data e hora de criação do TED.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CriadoEm { get; set; }

    /// <summary>
    /// Status do TED.
    /// </summary>
    [JsonPropertyName("status")]
    [EnumDataType(typeof(StatusEnum))]
    public StatusEnum Status { get; set; }

    /// <summary>
    /// ID de confirmação Sinacor (ignorado na serialização JSON).
    /// </summary>
    [JsonPropertyName("sinacorConfirmationId")]
    public string? SinacorConfirmacaoId { get; set; }

    /// <summary>
    /// Motivo da Regeição.
    /// </summary>
    [JsonPropertyName("motiveRejection")]
    public string? MotivoReprovacao { get; set; }
}