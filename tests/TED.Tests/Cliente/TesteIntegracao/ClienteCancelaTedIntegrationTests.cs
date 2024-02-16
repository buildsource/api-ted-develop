using System.Net.Http.Json;
using TED.API.DTOs;
using TED.API.Response;
using Xunit.Extensions.Ordering;
using System.Text.Json;
using TED.API.Enums;
using Xunit;

namespace TED.Tests.Cliente.TesteIntegracao;

[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class ClienteCancelaTedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _url = "/api/v1/cliente";
    private ClienteTedRequestDto _tedDto;

    public ClienteCancelaTedIntegrationTests(
        CustomWebApplicationFactory factory
    )
    {
        _client = factory.CreateClient();


        //Arrange
        _tedDto = new ClienteTedRequestDto
        {
            ClienteId = new Random().Next(1, 998),
            DataAgendamento = DateTime.Now,
            ValorSolicitado = 10000000,
            NumeroAgencia = "1234",
            NumeroConta = "123456",
            DigitoConta = "7",
            NumeroBanco = "123"
        };
    }

    [Fact(DisplayName = "Cancela TED com ID Válido e Retorna Sucesso"), Order(1)]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTed_ComIdValido_RetornaSucesso()
    {
        // Arrange
        await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);

        var response = await _client.GetAsync($"{_url}/lista-ted/{_tedDto.ClienteId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        var id = tedList?.Data?.Items?.FirstOrDefault()?.Id;

        // Act
        response = await _client.PutAsync($"{_url}/cancela-ted/{id}", null);
        content = await response.Content.ReadAsStringAsync();
        var cancelamentoResponse = JsonSerializer.Deserialize<ApiResponse<ClienteTedResponseDto>>(content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(cancelamentoResponse);
        Assert.True(cancelamentoResponse.IsSuccess);
        Assert.Equal("Ted cancelado com sucesso", cancelamentoResponse.Message);
    }

    [Fact(DisplayName = "Cancela TED com ID Inválido e Retorna Erro de Validação"), Order(2)]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTed_ComIdInvalido_LancaExcecaoDeValidacao()
    {
        // Arrange
        var clienteIdInvalido = -1; // ID inválido

        // Act & Assert
        var response = await _client.PutAsync($"{_url}/cancela-ted/{clienteIdInvalido}", null);

        Assert.False(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var cancelamentoResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);
        Assert.NotNull(cancelamentoResponse);
        Assert.False(cancelamentoResponse.IsSuccess);
        Assert.Equal("Erro na validação", cancelamentoResponse.Message);
        foreach (var item in cancelamentoResponse.Data)
        {
            Assert.Equal("O Id não é válido", item.Message);
        }
    }

    [Fact(DisplayName = "Cancela TED com ID Válido Com Status Aprovado e Retorna Erro de Validação"), Order(3)]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTed_ComIdValidoEStatusApproved_LancaExcecaoDeValidacao()
    {
        // Arrange
        _tedDto.ValorSolicitado = 10;

        var response = await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);
        response.EnsureSuccessStatusCode();

        response = await _client.GetAsync($"{_url}/lista-ted/{_tedDto.ClienteId}?status={StatusEnum.Approved}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        var id = tedList?.Data?.Items?.FirstOrDefault()?.Id;

        // Act & Assert
        response = await _client.PutAsync($"{_url}/cancela-ted/{id}", null);

        Assert.False(response.IsSuccessStatusCode);

        content = await response.Content.ReadAsStringAsync();
        var cancelamentoResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);

        Assert.NotNull(cancelamentoResponse);
        Assert.False(cancelamentoResponse.IsSuccess);
        Assert.Equal("Ocorreu um erro ao cancelar o Ted", cancelamentoResponse.Message);
        foreach (var item in cancelamentoResponse.Data)
        {
            Assert.Equal("Ted não pode mais ser cancelardo", item.Message);
        }
    }

    [Fact(DisplayName = "Cancela TED com ID Válido Com Status Cancelado e Retorna Erro de Validação"), Order(4)]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTed_ComIdValidoEStatusCanceled_LancaExcecaoDeValidacao()
    {
        // Arrange
        _tedDto.ValorSolicitado = 100000;

        var response = await _client.PostAsJsonAsync($"{_url}/solicita-ted", _tedDto);
        response.EnsureSuccessStatusCode();

        response = await _client.GetAsync($"{_url}/lista-ted/{_tedDto.ClienteId}?status={StatusEnum.InProcess}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

        var id = tedList?.Data?.Items?.FirstOrDefault()?.Id;

        await _client.PutAsync($"{_url}/cancela-ted/{id}", null);
        response.EnsureSuccessStatusCode();

        // Act & Assert
        response = await _client.PutAsync($"{_url}/cancela-ted/{id}", null);

        Assert.False(response.IsSuccessStatusCode);

        content = await response.Content.ReadAsStringAsync();
        var cancelamentoResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);

        Assert.NotNull(cancelamentoResponse);
        Assert.False(cancelamentoResponse.IsSuccess);
        Assert.Equal("Ocorreu um erro ao cancelar o Ted", cancelamentoResponse.Message);
        foreach (var item in cancelamentoResponse.Data)
        {
            Assert.Equal("Ted já foi cancelardo", item.Message);
        }
    }
}
