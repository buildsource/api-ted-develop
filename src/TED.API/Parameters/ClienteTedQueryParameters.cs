using Microsoft.AspNetCore.Mvc;
using TED.API.Enums;

namespace TED.API.Parameters;

/// <summary>
/// Parâmetros de consulta para a busca de TEDs.
/// </summary>

public class ClienteTedQueryParameters
{
    /// <summary>
    /// Data de início para filtrar TEDs.
    /// </summary>
    [FromQuery(Name = "DataInicio")]
    public DateTime? DataInicio { get; set; }

    /// <summary>
    /// Data de fim para filtrar TEDs.
    /// </summary>
    [FromQuery(Name = "DataFim")]
    public DateTime? DataFim { get; set; }

    /// <summary>
    /// Status do TED para filtragem.
    /// 0 - Em processo de aprovação, 1 - Status aprovado, 2 - Status cancelado, 3 - Status reprovado
    /// </summary>
    [FromQuery(Name = "Status")]
    public StatusEnum? Status { get; set; }

    /// <summary>
    /// Número da página para a consulta de paginação. Padrão é 1.
    /// </summary>
    [FromQuery(Name = "NumeroPagina")]
    public int NumeroPagina { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página para a consulta de paginação. Padrão é 10.
    /// </summary>
    [FromQuery(Name = "QuantidadeItensPagina")]
    public int QuantidadeItensPagina { get; set; } = 10;
}
