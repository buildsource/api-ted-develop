using System.Net.Http.Json;
using System.Text.Json;
using TED.API.DTOs;
using TED.API.Enums;
using TED.API.Response;
using Xunit;
using Xunit.Extensions.Ordering;

namespace TED.Tests.Admin.TesteIntegracao { 

    [TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
    public class AdminReprovaTedIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _url = "/api/v1";
        private ClienteTedRequestDto _tedDto;

        public AdminReprovaTedIntegrationTests(
            CustomWebApplicationFactory factory
        )
        {
            _client = factory.CreateClient();


            //Arrange
            _tedDto = new ClienteTedRequestDto
            {
                ClienteId = new Random().Next(1, 998),
                DataAgendamento = DateTime.Now,
                ValorSolicitado = 100.00,
                NumeroAgencia = "1234",
                NumeroConta = "1234567",
                DigitoConta = "0",
                NumeroBanco = "123"
            };
        }

        [Fact(DisplayName = "Reprova TED com ID Válido e Retorna Sucesso"), Order(1)]
        [Trait("Categoria", "AdminReprovaTed")]
        public async Task ReprovaTed_ComIdValido_RetornaSucesso()
        {
            // Arrange
            _tedDto.ValorSolicitado = 100000;

            await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);

            var response = await _client.GetAsync($"{_url}/cliente/lista-ted/{_tedDto.ClienteId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

            var id = tedList?.Data.Items?.FirstOrDefault()?.Id;

            var adminTedRequestDto = new AdminTedRequestDto
            {
                MotivoReprovacao = "Motivo reprovação aqui"
            };

            // Act
            response = await _client.PutAsJsonAsync($"{_url}/admin/reprova-ted/{id}", adminTedRequestDto);

            // Assert
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            var tedResponse = JsonSerializer.Deserialize<ApiResponse<AdminTedResponseDto>>(content);

            Assert.NotNull(tedResponse);
            Assert.True(tedResponse.IsSuccess);
            Assert.Equal("Ted reprovado com sucesso", tedResponse.Message);
        }

        [Fact(DisplayName = "Reprova TED com ID Inválido e Retorna Erro de Validação"), Order(2)]
        [Trait("Categoria", "AdminReprovaTed")]
        public async Task ReprovaTed_ComIdInvalido_RetornaErroValidacao()
        {
            // Arrange
            var idInvalido = -1;
            var adminTedRequestDto = new AdminTedRequestDto
            {
                MotivoReprovacao = "Motivo reprovação aqui"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{_url}/admin/reprova-ted/{idInvalido}", adminTedRequestDto);

            // Assert
            Assert.NotEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var tedResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);

            Assert.NotNull(tedResponse);
            Assert.False(tedResponse.IsSuccess);
            Assert.Equal("Erro na validação", tedResponse.Message);
            foreach (var item in tedResponse.Data)
            {
                Assert.Equal("O Id não é válido", item.Message);
            }
        }


        [Fact(DisplayName = "Reprova TED com ID Válido Com Status Aprovado e Retorna Erro de Validação"), Order(3)]
        [Trait("Categoria", "AdminReprovaTed")]
        public async Task ReprovaTed_ComIdValidoEStatusApproved_LancaExcecaoDeValidacao()
        {
            // Arrange
            _tedDto.ValorSolicitado = 10;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync($"{_url}/cliente/lista-ted/{_tedDto.ClienteId}?status={StatusEnum.Approved}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

            var id = tedList?.Data?.Items?.FirstOrDefault()?.Id;

            var adminTedRequestDto = new AdminTedRequestDto
            {
                MotivoReprovacao = "Motivo reprovação aqui"
            };

            // Act & Assert
            response = await _client.PutAsJsonAsync($"{_url}/admin/reprova-ted/{id}", adminTedRequestDto);

            Assert.False(response.IsSuccessStatusCode);

            content = await response.Content.ReadAsStringAsync();
            var cancelamentoResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);

            Assert.NotNull(cancelamentoResponse);
            Assert.False(cancelamentoResponse.IsSuccess);
            Assert.Equal("Ocorreu um erro ao reprovar o Ted", cancelamentoResponse.Message);
            foreach (var item in cancelamentoResponse.Data)
            {
                Assert.Equal("Ted não pode mais ser reprovado", item.Message);
            }
        }

        [Fact(DisplayName = "Reprova TED com ID Válido Com Status Cancelado e Retorna Erro de Validação"), Order(4)]
        [Trait("Categoria", "AdminReprovaTed")]
        public async Task ReprovaTed_ComIdValidoEStatusCanceled_LancaExcecaoDeValidacao()
        {
            // Arrange
            _tedDto.ValorSolicitado = 10000000;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            response = await _client.GetAsync($"{_url}/cliente/lista-ted/{_tedDto.ClienteId}?status={StatusEnum.InProcess}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<ClienteTedResponseDto>>>(content);

            var id = tedList?.Data?.Items?.FirstOrDefault()?.Id;

            await _client.PutAsync($"{_url}/cliente/cancela-ted/{id}", null);
            response.EnsureSuccessStatusCode();

            var adminTedRequestDto = new AdminTedRequestDto
            {
                MotivoReprovacao = "Motivo reprovação aqui"
            };

            // Act & Assert
            response = await _client.PutAsJsonAsync($"{_url}/admin/reprova-ted/{id}", adminTedRequestDto);

            Assert.False(response.IsSuccessStatusCode);

            content = await response.Content.ReadAsStringAsync();
            var cancelamentoResponse = JsonSerializer.Deserialize<ApiResponse<List<Notification>>>(content);

            Assert.NotNull(cancelamentoResponse);
            Assert.False(cancelamentoResponse.IsSuccess);
            Assert.Equal("Ocorreu um erro ao reprovar o Ted", cancelamentoResponse.Message);
            foreach (var item in cancelamentoResponse.Data)
            {
                Assert.Equal("Ted não pode mais ser reprovado", item.Message);
            }
        }
    }
}
