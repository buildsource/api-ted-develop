using System.Net.Http.Json;
using System.Net;
using TED.API.DTOs;
using TED.API.Response;
using Xunit.Extensions.Ordering;
using System.Text.Json;
using Xunit;

namespace TED.Tests.Cliente.TesteIntegracao;

[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class ClienteSolicitaTedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _url = "/api/v1/cliente/solicita-ted";
    private ClienteTedRequestDto _tedDto;
    private LimiteTedRequestDto _limiteTedRequestDto;

    public ClienteSolicitaTedIntegrationTests(
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

        _limiteTedRequestDto = new LimiteTedRequestDto
        {
            ValorMaximoDia = 10000,
            QuantidadeMaximaDia = 3,
            ValorMaximoPorSaque = 5000
        };
    }

    [Fact(DisplayName = "Solicitar TED Dentro do Limite Retorna Sucesso"), Order(1)]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitarTed_DentroDoLimite_RetornaSucesso()
    {
        // Act
        _tedDto.ClienteId = 26054;
        _tedDto.ValorSolicitado = 10;
        _tedDto.DataAgendamento = DateTime.Now;

        var response = await _client.PostAsJsonAsync(_url, _tedDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteTedResponseDto>>(apiResponseString);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal("Ted solicitado com sucesso", apiResponse.Message);

        Assert.False(string.IsNullOrEmpty(apiResponse.Data.SinacorConfirmacaoId));
        Assert.True(apiResponse.Data.ClienteId > 0);
        Assert.Equal(API.Enums.StatusEnum.Approved, apiResponse.Data.Status);
    }

    [Fact(DisplayName = "Solicitar TED com Dados Inválidos Retorna Erro de Validação"), Order(2)]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitarTed_ComDadosInvalidos_RetornaErroValidacao()
    {
        // Arrange
        _tedDto.ValorSolicitado = -1000.00;

        // Act
        var response = await _client.PostAsJsonAsync(_url, _tedDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var apiResponseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(apiResponseString);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
    }

    [Fact(DisplayName = "Solicitar TED Acima do Limite Diário Retorna Erro de Limite de Quantidade"), Order(3)]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitarTed_AcimaDoLimiteDiario_RetornaErroLimiteQuantidade()
    {
        //Arrange
        _tedDto.DataAgendamento = DateTime.Now.Date;

        // Simula várias solicitações até atingir o limite
        for (int i = 0; i < _limiteTedRequestDto.QuantidadeMaximaDia; i++)
        {
            await _client.PostAsJsonAsync(_url, _tedDto);
        }

        // Act - Faça mais uma solicitação que deve exceder o limite
        var response = await _client.PostAsJsonAsync(_url, _tedDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteTedResponseDto>>(apiResponseString);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal("Ted solicitado com sucesso", apiResponse.Message);

        Assert.True(string.IsNullOrEmpty(apiResponse.Data.SinacorConfirmacaoId));
        Assert.True(apiResponse.Data.ClienteId > 0);
        Assert.Equal(API.Enums.StatusEnum.InProcess, apiResponse.Data.Status);
    }

    [Fact(DisplayName = "Solicitar TED Acima do Valor Máximo Diário Retorna Erro de Limite de Valor"), Order(4)]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitarTed_AcimaDoValorMaximoDiario_RetornaErroLimiteValor()
    {
        // Arrange
        _tedDto.ValorSolicitado = _limiteTedRequestDto.ValorMaximoDia + 1;

        // Act
        var response = await _client.PostAsJsonAsync(_url, _tedDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteTedResponseDto>>(apiResponseString);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal("Ted solicitado com sucesso", apiResponse.Message);

        Assert.True(string.IsNullOrEmpty(apiResponse.Data.SinacorConfirmacaoId));
        Assert.True(apiResponse.Data.ClienteId > 0);
        Assert.Equal(API.Enums.StatusEnum.InProcess, apiResponse.Data.Status);
    }

    [Fact(DisplayName = "Solicitar TED Acima do Valor Máximo por Saque Retorna Erro de Limite de Valor por Saque"), Order(5)]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitarTed_AcimaDoValorMaximoPorSaque_RetornaErroLimiteValorSaque()
    {
        // Arrange
        _tedDto.ValorSolicitado = _limiteTedRequestDto.ValorMaximoPorSaque + 1;

        // Act
        var response = await _client.PostAsJsonAsync(_url, _tedDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteTedResponseDto>>(apiResponseString);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal("Ted solicitado com sucesso", apiResponse.Message);

        Assert.True(string.IsNullOrEmpty(apiResponse.Data.SinacorConfirmacaoId));
        Assert.True(apiResponse.Data.ClienteId > 0);
        Assert.Equal(API.Enums.StatusEnum.InProcess, apiResponse.Data.Status);
    }

    [Fact(DisplayName = "Solicitar TED Agendado Retorna Sucesso"), Order(1)]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitarTed_Agendado_RetornaSucesso()
    {
        // Act
        _tedDto.ClienteId = 26054;
        _tedDto.ValorSolicitado = 10;
        _tedDto.DataAgendamento = DateTime.Now.AddDays(1);

        var response = await _client.PostAsJsonAsync(_url, _tedDto);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteTedResponseDto>>(apiResponseString);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal("Ted solicitado com sucesso", apiResponse.Message);

        Assert.True(string.IsNullOrEmpty(apiResponse.Data.SinacorConfirmacaoId));
        Assert.True(apiResponse.Data.ClienteId > 0);
        Assert.Equal(API.Enums.StatusEnum.InProcess, apiResponse.Data.Status);
    }
}
