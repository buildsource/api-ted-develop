using System.Text.Json.Serialization;
using TED.API.Annotations;

namespace TED.API.DTOs;

/// <summary>
/// Representa os dados de requisição para atualização dos limites de TED.
/// </summary>
[IncludeInSwagger]
public class LimiteTedRequestDto
{
    /// <summary>
    /// Obtém ou define o valor máximo permitido por dia para TED.
    /// </summary>
    [JsonPropertyName("valorMaximoDia")]
    public double ValorMaximoDia { get; set; }

    /// <summary>
    /// Obtém ou define a quantidade máxima de TEDs permitidas por dia.
    /// </summary>
    [JsonPropertyName("quantidadeMaximaDia")]
    public int QuantidadeMaximaDia { get; set; }

    /// <summary>
    /// Obtém ou define o valor máximo permitido por TED individual.
    /// </summary>
    [JsonPropertyName("valorMaximoPorSaque")]
    public double ValorMaximoPorSaque { get; set; }
}
