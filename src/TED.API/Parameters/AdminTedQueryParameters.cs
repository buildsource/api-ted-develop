using Microsoft.AspNetCore.Mvc;

namespace TED.API.Parameters;

/// <summary>
/// Parâmetros de consulta para a busca de TEDs na perspectiva administrativa.
/// </summary>

public class AdminTedQueryParameters: ClienteTedQueryParameters
{
    /// <summary>
    /// ID do cliente para filtrar TEDs.
    /// </summary>
    [FromQuery(Name = "ClienteId")]
    public int? ClienteId { get; set; }

    /// <summary>
    ///Nome do cliente para filtrar TEDs.
    /// </summary>
    [FromQuery(Name = "NomeCliente")]
    public string? NomeCliente { get; set; }

    /// <summary>
    /// Nome do banco para filtrar TEDs.
    /// </summary>
    [FromQuery(Name = "NomeBanco")]
    public string? NomeBanco { get; set; }
}
