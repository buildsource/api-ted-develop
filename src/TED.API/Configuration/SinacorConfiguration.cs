using System.Text.Json.Serialization;

public class SinacorConfiguration
{
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("clienteSecret")]
    public string ClienteSecret { get; set; }

    [JsonPropertyName("clienteId")]
    public string ClienteId { get; set; }

    [JsonPropertyName("horarioInicio")]
    public DateTime HorarioInicio { get; set; }

    [JsonPropertyName("horarioFim")]
    public DateTime HorarioFim { get; set; }

    [JsonPropertyName("isLocal")]
    public bool IsLocal { get; set; }
}