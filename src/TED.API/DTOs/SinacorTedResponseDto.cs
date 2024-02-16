using System.Text.Json.Serialization;
using TED.API.Annotations;

namespace TED.API.DTOs;

[IncludeInSwagger]
public class SinacorTedResponseDto
{
    [JsonPropertyName("sucesso")]
    public bool Sucesso { get; set; }

    [JsonPropertyName("protocolo")]
    public long Protocolo { get; set; }

    [JsonPropertyName("inconsistenciasRequest")]
    public List<InconsistenciasRequestDto> InconsistenciasRequest { get; set; }

    [JsonPropertyName("inconsistenciasLancamentos")]
    public List<InconsistenciasLancamentoDto> InconsistenciasLancamentos { get; set; }

}

public class InconsistenciasRequestDto
{
    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }

    [JsonPropertyName("codigo")]
    public long Codigo { get; set; }
}

public class InconsistenciasLancamentoDto
{
    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }

    [JsonPropertyName("idErro")]
    public int ErroId { get; set; }

    [JsonPropertyName("codigo")]
    public long Codigo { get; set; }
}