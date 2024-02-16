using System.Net.Http.Json;
using TED.API.DTOs;
using Xunit.Extensions.Ordering;
using TED.API.Response;
using System.Text.Json;
using TED.API.Enums;
using System.Text;
using System.Net;
using Xunit;

namespace TED.Tests.Cliente.TesteIntegracao;

[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class ClienteObtemListaTedPaginadaIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _url = "/api/v1/cliente";
    private ClienteTedRequestDto _tedDto;

    public ClienteObtemListaTedPaginadaIntegrationTests(
        CustomWebApplicationFactory factory
    )
    {
        _client = factory.CreateClient();

        //Arrange
        _tedDto = new ClienteTedRequestDto
        {
            ClienteId = new Random().Next(1, 998),
            DataAgendamento = DateTime.Now,
            ValorSolicitado = 1000.00,
            NumeroAgencia = "0001",
            NumeroConta = "26054",
            DigitoConta = "0",
            NumeroBanco = "83"
        };
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID Válido Retorna Lista Não Vazia"), Order(1)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdValido_RetornaListaNaoVazia()
    {
        // Arrange
        var statusEspecifico = StatusEnum.Approved;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?Status={statusEspecifico}");


        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count > 0);
        Assert.True(tedList.Data.TotalItems > 0);
        foreach (var ted in tedList.Data.Items)
        {
            Assert.Equal(statusEspecifico, ted.Status);
        }
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID Inválido Retorna Erro de Validação"), Order(2)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdInvalido_RetornaErroValidacao()
    {
        // Arrange
        var clienteIdInvalido = -1; // Um ID inválido, por exemplo, um número negativo
        var url = $"{_url}/lista-ted/{clienteIdInvalido}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);

        Assert.False(apiResponse.IsSuccess);
        Assert.NotNull(apiResponse.Data);
        Assert.Contains("Erro na validação", apiResponse.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID Sem TEDs Associados Retorna Lista Vazia"), Order(3)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdSemTeds_RetornaListaVazia()
    {
        // Arrange
        var clienteIdSemTeds = 9999; // Um ID que não tem TEDs associados
        var url = $"{_url}/lista-ted/{clienteIdSemTeds}";

        // Act
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count == 0);
        Assert.True(tedList.Data.TotalItems == 0);
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID e Status Válidos Retorna Lista Não Vazia Filtrada"), Order(4)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdEStatusValidos_RetornaListaNaoVaziaFiltrada()
    {
        // Arrange
        _tedDto.ValorSolicitado = 10;
        var statusEspecifico = StatusEnum.Approved;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?Status={(int)statusEspecifico}");

        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count > 0);
        Assert.True(tedList.Data.TotalItems > 0);
        foreach (var ted in tedList.Data.Items)
        {
            Assert.Equal(statusEspecifico, ted.Status);
        }
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID e Status Sem TEDs Associados Retorna Lista Vazia"), Order(5)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdEStatusSemTeds_RetornaListaVazia()
    {
        // Arrange
        var statusEspecifico = StatusEnum.Canceled;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?status={(int)statusEspecifico}");

        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count == 0);
        Assert.True(tedList.Data.TotalItems == 0);
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID e Data de Início Válidos Retorna Lista Não Vazia Filtrada"), Order(6)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdEDataInicioValidos_RetornaListaNaoVaziaFiltrada()
    {
        // Arrange
        var dataInicioEspecifica = DateTime.Now.ToString("yyyy-MM-dd");

        _tedDto.ValorSolicitado = 100000;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?startDate={dataInicioEspecifica}");

        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count > 0);
        Assert.True(tedList.Data.TotalItems > 0);
        foreach (var ted in tedList.Data.Items)
        {
            Assert.True(ted.CriadoEm.Date >= DateTime.Parse(dataInicioEspecifica).Date);
        }
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID e Data de Início Sem TEDs Associados Retorna Lista Vazia"), Order(7)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdEDataInicioSemTeds_RetornaListaVazia()
    {
        // Arrange
        var dataInicioEspecifica = DateTime.Now.ToString("yyyy-MM-dd");

        _tedDto.ValorSolicitado = 10;

        var response = await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);
        response.EnsureSuccessStatusCode();

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?startDate={dataInicioEspecifica}");

        // Act
        response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count == 0);
        Assert.True(tedList.Data.TotalItems == 0);
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID, Data de Início e Fim Válidos Retorna Lista Não Vazia Filtrada"), Order(8)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdDataInicioEDataFimValidos_RetornaListaNaoVaziaFiltrada()
    {
        // Arrange
        var dataInicioEspecifica = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
        var dataFimEspecifica = DateTime.Now.ToString("yyyy-MM-dd");

        _tedDto.ValorSolicitado = 100000;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?startDate={dataInicioEspecifica}");
        urlBuilder.Append($"&endDate={dataFimEspecifica}");


        // Act
        var response = await _client.GetAsync(urlBuilder.ToString()) ;
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count > 0);
        Assert.True(tedList.Data.TotalItems > 0);
        foreach (var ted in tedList.Data.Items)
        {
            Assert.True(ted.CriadoEm.Date >= DateTime.Parse(dataInicioEspecifica).Date && ted.CriadoEm.Date <= DateTime.Parse(dataFimEspecifica).Date);
        }
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID, Data de Início e Fim Sem TEDs Associados Retorna Lista Vazia"), Order(9)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdDataInicioEDataFimSemTeds_RetornaListaVazia()
    {
        // Arrange
        var dataInicioEspecifica = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
        var dataFimEspecifica = DateTime.Now.AddDays(-25).ToString("yyyy-MM-dd");

        _tedDto.ValorSolicitado = 100000;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?startDate={dataInicioEspecifica}");
        urlBuilder.Append($"&endDate={dataFimEspecifica}");

        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count == 0);
        Assert.True(tedList.Data.TotalItems == 0);
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID, Status, Data de Início e Fim Válidos Retorna Lista Não Vazia Filtrada"), Order(10)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdStatusDataInicioEDataFimValidos_RetornaListaNaoVaziaFiltrada()
    {
        // Arrange
        var dataInicioEspecifica = DateTime.Now.ToString("yyyy-MM-dd");
        var dataFimEspecifica = DateTime.Now.ToString("yyyy-MM-dd");
        var statusEspecifico = StatusEnum.Approved;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?startDate={dataInicioEspecifica}");
        urlBuilder.Append($"&endDate={dataFimEspecifica}");
        urlBuilder.Append($"&status={(int)statusEspecifico}");

        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count > 0);
        Assert.True(tedList.Data.TotalItems > 0);
        foreach (var ted in tedList.Data.Items)
        {
            Assert.True(ted.CriadoEm.Date >= DateTime.Parse(dataInicioEspecifica).Date && ted.CriadoEm.Date <= DateTime.Parse(dataFimEspecifica).Date);
            Assert.Equal(statusEspecifico, ted.Status);
        }
    }

    [Fact(DisplayName = "Obtém Lista TED Paginada com Cliente ID, Status, Data de Início e Fim Sem TEDs Associados Retorna Lista Vazia"), Order(11)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginada_ComClienteIdStatusDataInicioEDataFimSemTeds_RetornaListaVazia()
    {
        // Arrange
        var dataInicioEspecifica = DateTime.Now.ToString("yyyy-MM-dd");
        var dataFimEspecifica = DateTime.Now.ToString("yyyy-MM-dd");
        var statusEspecifico = StatusEnum.Canceled;

        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?startDate={dataInicioEspecifica}");
        urlBuilder.Append($"&endDate={dataFimEspecifica}");
        urlBuilder.Append($"&status={(int)statusEspecifico}");

        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.Items.Count == 0);
        Assert.True(tedList.Data.TotalItems == 0);
    }


    [Fact(DisplayName = "Obtem Lista TED Paginada Paginada pelo NumeroPagina e QuantidadeItensPagina e Retorna Lista Não Vazia"), Order(14)]
    [Trait("Categoria", "ClienteObtemListaTedPaginada")]
    public async Task ObtemListaTedPaginadaPeloNumeroPaginaEQuantidadeItensPagina_RetornaListaNaoVazia()
    {
        // Arrange
        var numeroPagina = 1;
        var quantidadeItensPagina = 5;

        for (int i = 0; i < 100; i++)
        {
            var solicitaResponse = await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);
            solicitaResponse.EnsureSuccessStatusCode();
        }

        var urlBuilder = new StringBuilder($"{_url}/lista-ted/{_tedDto.ClienteId}");
        urlBuilder.Append($"?pageNumber={numeroPagina}");
        urlBuilder.Append($"&pageSize={quantidadeItensPagina}");


        // Act
        var response = await _client.GetAsync(urlBuilder.ToString());
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        // Assert
        Assert.True(tedList.IsSuccess);
        Assert.True(tedList.Data.CurrentPage == numeroPagina);
        Assert.True(tedList.Data.Items.Count == quantidadeItensPagina);
        Assert.True(tedList.Data.PageSize == quantidadeItensPagina);
        Assert.True(tedList.Data.TotalItems == tedList.Data.TotalPages * quantidadeItensPagina);
    }
}
