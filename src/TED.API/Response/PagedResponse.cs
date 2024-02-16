using System.Text.Json.Serialization;

namespace TED.API.Response;

/// <summary>
/// Representa uma resposta paginada genérica para uma consulta de API.
/// Esta classe é utilizada para encapsular os dados de uma lista paginada,
/// incluindo informações detalhadas sobre a paginação, como o número total de itens,
/// o número total de páginas, a página atual e o tamanho da página.
/// </summary>
/// <typeparam name="T">O tipo de dados contidos na lista de itens.</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Lista dos itens do tipo <typeparamref name="T"/> na página atual.
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; }

    /// <summary>
    /// Número total de itens em todas as páginas.
    /// </summary>
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    /// <summary>
    /// Número total de páginas disponíveis.
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Número da página atual.
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Número de itens por página.
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="PagedResponse{T}"/> com os parâmetros especificados.
    /// </summary>
    /// <param name="items">Os itens da página atual.</param>
    /// <param name="count">O número total de itens em todas as páginas.</param>
    /// <param name="pageNumber">O número da página atual.</param>
    /// <param name="pageSize">O número de itens por página.</param>
    public PagedResponse(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalItems = count;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public PagedResponse() { }
}
