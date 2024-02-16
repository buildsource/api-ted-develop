using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TED.API.Enums;

namespace TED.API.DTOs;

public class AdminTedResponseDto : AdminTedRequestDto
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
    /// ID do cliente que está solicitando o TED.
    /// </summary>
    [JsonPropertyName("clientId")]
    public int ClienteId { get; set; }

    /// <summary>
    ///Nome do cliente que está solicitando o TED.
    /// </summary>
    [JsonPropertyName("clientName")]
    public string NomeCliente { get; set; }

    /// <summary>
    /// Data agendada para a realização do TED.
    /// </summary>
    [JsonPropertyName("schedulingDate")]
    public DateTime DataAgendamento { get; set; }

    /// <summary>
    /// Valor solicitado para o TED.
    /// </summary>
    [JsonPropertyName("requestedValue")]
    public double ValorSolicitado { get; set; }

    /// <summary>
    /// Número da agência do cliente.
    /// </summary>
    [JsonPropertyName("numberAgency")]
    public string NumeroAgencia { get; set; }

    /// <summary>
    /// Número da conta do cliente.
    /// </summary>
    [JsonPropertyName("accountNumber")]
    public string NumeroConta { get; set; }

    /// <summary>
    /// Número do banco para o TED.
    /// </summary>
    [JsonPropertyName("bankNumber")]
    public string NumeroBanco { get; set; }

    /// <summary>
    /// Nome do banco para o TED.
    /// </summary>
    [JsonPropertyName("bankName")]
    public string NomeBanco { get; set; }

    /// <summary>
    /// ID de confirmação Sinacor (ignorado na serialização JSON).
    /// </summary>
    [JsonPropertyName("sinacorConfirmacaoId")]
    public string? SinacorConfirmacaoId { get; set; }
}