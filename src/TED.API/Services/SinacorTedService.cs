using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TED.API.DTOs;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Response;
using TED.API.Validators;

namespace TED.API.Services;

public class SinacorTedService: ISinacorTedService
{
    private readonly HttpClient _httpClient;
    private readonly SinacorConfiguration _sinacorConfiguration;


    public SinacorTedService(
        IHttpClientFactory httpClientFactory,
        IOptions<SinacorConfiguration> sinacorConfiguration
    )
    {
        _sinacorConfiguration = sinacorConfiguration.Value;

        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(_sinacorConfiguration.BaseUrl);
    }

    private async Task<string> ObtemTokenAsync()
    {
        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _sinacorConfiguration.ClienteId),
                new KeyValuePair<string, string>("client_secret", _sinacorConfiguration.ClienteSecret)
            });

            var response = await _httpClient.PostAsync("/infra/api/v1/oauth/token", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao obter o token: {response.ReasonPhrase}");


            var jsonResponse = await response.Content.ReadAsStringAsync();


            var tokenResponse = JsonSerializer.Deserialize<SinacorTokenResponse>(jsonResponse);

            return tokenResponse.AccessToken;
        }
        catch ( Exception _)
        {
            throw;
        }
    }

    public async Task<string?> ClienteEnviaTedParaSinacorAsync(SinacorTedRequestDto sinacorTedRequestDto)
    {
        var validator = new SinacorTedRequestDtoValidator();

        var validationResult = validator.Validate(sinacorTedRequestDto);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors.Select(e => e.ErrorMessage.TrimEnd('.')).ToList());

        if (_sinacorConfiguration.IsLocal)
            return new Random().Next(111, 999).ToString();

        try
        {
            var token = await ObtemTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("COMPANY_ID", "83");
            _httpClient.DefaultRequestHeaders.Add("SISTEMA_ORIGEM", "CCOLE");

            var jsonRequest = JsonSerializer.Serialize(sinacorTedRequestDto);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/tesouraria/api/v2/LancamentosTES", content);


            var jsonResponse = await response.Content.ReadAsStringAsync();
            var sinacorResponse = JsonSerializer.Deserialize<SinacorTedResponseDto>(jsonResponse);


            if (!response.IsSuccessStatusCode)
                throw new ValidationException(
                    sinacorResponse?.InconsistenciasLancamentos?.Select(e => e.Descricao.TrimEnd('.')).ToList()
                    ?? 
                    sinacorResponse?.InconsistenciasRequest?.Select(e => e.Descricao.TrimEnd('.')).ToList()
                );

            return sinacorResponse?.Protocolo.ToString();
        }
        catch (Exception _)
        {
            throw;
        }
    }
}