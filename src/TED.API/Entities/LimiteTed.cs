using System.Text.Json.Serialization;

namespace TED.API.Entities;

/// <summary>
/// Representa os limites operacionais para transações TED, incluindo valores máximos por dia e por saque.
/// </summary>
public class LimiteTed : BaseEntity
{
    /// <summary>
    /// Obtém ou define o valor máximo permitido para transações TED em um único dia.
    /// </summary>
    [JsonPropertyName("valorMaximoDia")]
    public double ValorMaximoDia { get; set; }

    /// <summary>
    /// Obtém ou define a quantidade máxima de transações TED permitidas em um único dia.
    /// </summary>
    [JsonPropertyName("quantidadeMaximaDia")]
    public int QuantidadeMaximaDia { get; set; }

    /// <summary>
    /// Obtém ou define o valor máximo permitido para uma única transação TED.
    /// </summary>
    [JsonPropertyName("valorMaximoPorSaque")]
    public double ValorMaximoPorSaque { get; set; }
}
