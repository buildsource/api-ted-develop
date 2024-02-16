using System.Text.Json.Serialization;

namespace TED.API.DTOs;

/// <summary>
/// DTO para solicitação de TED por um cliente.
/// </summary>
public class ClienteTedRequestDto
{
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
    /// Dígito da conta do cliente.
    /// </summary>
    [JsonPropertyName("accountDigit")]
    public string DigitoConta { get; set; }

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
}
