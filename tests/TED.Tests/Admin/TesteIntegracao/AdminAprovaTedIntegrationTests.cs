using System.Net.Http.Json;
using System.Text.Json;
using TED.API.DTOs;
using TED.API.Response;
using Xunit;
using Xunit.Extensions.Ordering;

namespace TED.Tests.Admin.TesteIntegracao
{
    [TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
    public class AdminAprovaTedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _url = "/api/v1";
        private ClienteTedRequestDto _tedDto;

        public AdminAprovaTedIntegrationTests(
            CustomWebApplicationFactory factory
        )
        {
            _client = factory.CreateClient();


            //Arrange
            _tedDto = new ClienteTedRequestDto
            {
                ClienteId = new Random().Next(1, 998),
                DataAgendamento = DateTime.Now.AddDays(1),
                ValorSolicitado = 100000,
                NumeroAgencia = "1234",
                NumeroConta = "1234567",
                DigitoConta = "0",
                NumeroBanco = "123"
            };
        }

        [Fact(DisplayName = "Aprova TED com ID válido e retorna sucesso"), Order(1)]
        [Trait("Categoria", "AdminAprovaTed")]
        public async Task AprovaTed_ComIdValido_RetornaSucesso()
        {
            // Arrange
            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync($"{_url}/cliente/lista-ted/{_tedDto.ClienteId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

            var id = tedList?.Data?.Items?.FirstOrDefault()?.Id;

            // Act
            response = await _client.PutAsync($"{_url}/admin/aprova-ted/{id}", null);

            // Assert
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            var tedResponse = JsonSerializer.Deserialize<ApiResponse<AdminTedResponseDto>>(content);

            Assert.NotNull(tedResponse);
            Assert.True(tedResponse.IsSuccess);
            Assert.Equal("Ted aprovado com sucesso", tedResponse.Message);
        }

        [Fact(DisplayName = "Aprova TED com ID inválido e retorna erro de validação"), Order(2)]
        [Trait("Categoria", "AdminAprovaTed")]
        public async Task AprovaTed_ComIdInvalido_RetornaErroDeValidacao()
        {
            // Arrange
            var tedIdInvalido = -1; // Um ID inválido para teste

            // Act
            var response = await _client.PutAsync($"{_url}/admin/aprova-ted/{tedIdInvalido}", null);

            // Assert
            Assert.False(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var erroResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);

            Assert.NotNull(erroResponse);
            Assert.False(erroResponse.IsSuccess);
            Assert.Equal("Erro na validação", erroResponse.Message);
        }
    }
}
