using System.Net.Http.Json;
using TED.API.DTOs;
using Xunit.Extensions.Ordering;
using System.Text.Json;
using TED.API.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using System.Net.Http.Headers;
using System.Text;
using TED.API.Response;
using System.Net;

namespace TED.Tests.Cliente.TesteIntegracao;

[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class ClienteEnviaTedParaSinacorIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly SinacorConfiguration _sinacorConfiguration;
    private SinacorTedRequestDto _sinacorTedRequestDto;

    public ClienteEnviaTedParaSinacorIntegrationTests(
        CustomWebApplicationFactory factory
    )
    {
        _sinacorConfiguration = factory.Services.GetRequiredService<IOptions<SinacorConfiguration>>().Value;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_sinacorConfiguration.BaseUrl);

        // Arrange
        _sinacorTedRequestDto = new SinacorTedRequestDto
        {
            Lancamentos = new List<Lancamento>
            {
                {
                    new Lancamento
                    {
                        IdLcto = 10,
                        DataMovimento = DateTime.Now.ToString("yyyy-MM-dd"),
                        DataReferencia = DateTime.Now.ToString("yyyy-MM-dd"),
                        CodigoCliente = 26054,
                        ValorLcto = 5,
                        DescricaoLcto = "TED",
                        CodigoGrupoLiquidacao = 17,
                        CodigoBanco = "83",
                        CodigoAgencia = "0001",
                        NumeroConta = 26054,
                        CodigoBancoCliente = "237",
                        CodigoAgenciaCliente = "0001",
                        NumeroContaCliente =  "123456",
                        DigitoContaCliente = "9",
                        TipoContaCliente = "CC",
                        IndicadorSituacao = "E",
                        CodigoSistemaExterno = "HBS",

                    }
                }
            }
        };
    }

    [Fact(DisplayName = "Envia TED para Sinacor com Solicitação Válida Retorna Sucesso"), Order(1)]
    [Trait("Categoria", "ClienteEnviaTedParaSinacor")]
    public async Task EnviaTedParaSinacor_ComSolicitacaoValida_RetornaSucesso()
    {
        if (_sinacorConfiguration.IsLocal)
            return;

        // Arrange
        var token = await ObtemTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Add("COMPANY_ID", "83");
        _httpClient.DefaultRequestHeaders.Add("SISTEMA_ORIGEM", "CCOLE");

        var jsonRequest = JsonSerializer.Serialize(_sinacorTedRequestDto);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync("/tesouraria/api/v2/LancamentosTES", stringContent);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var tedResponse = JsonSerializer.Deserialize<ApiResponse<SinacorTedResponseDto>>(content);

        Assert.NotNull(tedResponse);
        Assert.True(tedResponse.IsSuccess);
        Assert.NotNull(tedResponse.Data);
    }

    [Fact(DisplayName = "Envia TED para Sinacor com Solicitação Inválida Lança Exceção de Validação"), Order(2)]
    [Trait("Categoria", "ClienteEnviaTedParaSinacor")]
    public async Task EnviaTedParaSinacor_ComSolicitacaoInvalida_LancaExcecaoDeValidacao()
    {
        if (_sinacorConfiguration.IsLocal)
            return;

        // Arrange
        var token = await ObtemTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/tesouraria/api/v2/LancamentosTES", _sinacorTedRequestDto);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseContent);
        Assert.Contains("Ocorreu um erro na validação da WebAPI RegistraLancamentosTES", responseContent);
    }

    [Fact(DisplayName = "Gera para Sinacor Retorna Sucesso"), Order(3)]
    [Trait("Categoria", "ClienteEnviaTedParaSinacor")]
    public async Task GeraParaSinacor_RetornaSucesso()
    {
        if (_sinacorConfiguration.IsLocal)
            return;

        // Arrange
        var values = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _sinacorConfiguration.ClienteId),
            new KeyValuePair<string, string>("client_secret", _sinacorConfiguration.ClienteSecret)
        };

        var content = new FormUrlEncodedContent(values);

        // Act
        var response = await _httpClient.PostAsync("/infra/api/v1/oauth/token", content);

        //Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Gera para Sinacor e Causa Exceção Lança Exceção"), Order(4)]
    [Trait("Categoria", "ClienteEnviaTedParaSinacor")]
    public async Task GeraParaSinacor_CausaExcecao_LancaExcecao()
    {
        if (_sinacorConfiguration.IsLocal)
            return;

        // Arrange
        var values = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", ""),
            new KeyValuePair<string, string>("client_secret", "")
        };

        var content = new FormUrlEncodedContent(values);

        // Act
        var response = await _httpClient.PostAsync("/infra/api/v1/oauth/token", content);

        //Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async Task<string> ObtemTokenAsync()
    {
        try
        {
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _sinacorConfiguration.ClienteId),
                new KeyValuePair<string, string>("client_secret", _sinacorConfiguration.ClienteSecret)
            };

            // Criando o conteúdo para a requisição
            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync("/infra/api/v1/oauth/token", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao obter o token: {response.ReasonPhrase}");


            var jsonResponse = await response.Content.ReadAsStringAsync();


            var tokenResponse = JsonSerializer.Deserialize<SinacorTokenResponse>(jsonResponse);

            return tokenResponse.AccessToken;
        }
        catch (Exception _)
        {
            throw;
        }
    }
}
