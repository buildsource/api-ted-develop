using System.Net.Http.Json;
using TED.API.DTOs;
using Xunit.Extensions.Ordering;
using TED.API.Response;
using System.Text.Json;
using TED.API.Enums;
using System.Text;
using System.Net;
using Xunit;

namespace TED.Tests.Admin.TesteIntegracao
{
    [TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
    public class AdminObtemListaTedPaginadaIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _url = "/api/v1";
        private ClienteTedRequestDto _tedDto;

        public AdminObtemListaTedPaginadaIntegrationTests(
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
                NumeroAgencia = "0001",
                NumeroConta = "26054",
                DigitoConta = "0",
                NumeroBanco = "83"
            };
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada e Retorna Lista Não Vazia"), Order(1)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_RetornaListaNaoVazia()
        {
            // Arrange
            var statusEspecifico = StatusEnum.InProcess;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");


            // Act
            response = await _client.GetAsync(urlBuilder.ToString());
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            // Assert
            Assert.True(tedList.IsSuccess);
            Assert.True(tedList.Data.Items.Count > 0);
            Assert.True(tedList.Data.TotalItems > 0);
            foreach (var ted in tedList.Data.Items)
            {
                Assert.Equal(statusEspecifico, ted.Status);
            }
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada com Cliente ID Inválido Retorna Erro de Validação"), Order(2)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_ComClienteIdInvalido_RetornaErroValidacao()
        {
            // Arrange
            var clienteIdInvalido = -1;
            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={clienteIdInvalido}");

            // Act
            var response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content);

            Assert.False(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada com Cliente ID Válido Retorna Lista Não Vazia"), Order(3)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_ComClienteIdValido_RetornaListaNaoVazia()
        {
            // Arrange
            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            // Assert
            Assert.True(tedList.IsSuccess);
            Assert.NotNull(tedList.Data);
            Assert.True(tedList.Data.Items.Count > 0);
            Assert.True(tedList.Data.TotalItems > 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada com Cliente ID Sem TEDs Associados Retorna Lista Vazia"), Order(4)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_ComClienteIdSemTeds_RetornaListaVazia()
        {
            // Arrange
            var clienteIdSemTeds = 999; // ID que não tem TEDs associados
            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={clienteIdSemTeds}");

            // Act
            var response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count == 0);
            Assert.True(apiResponse.Data.TotalItems == 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Status Retorna Lista Não Vazia"), Order(5)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorStatus_RetornaListaNaoVazia()
        {
            // Arrange
            var statusEspecifico = StatusEnum.InProcess;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&status={statusEspecifico}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count > 0);
            Assert.True(apiResponse.Data.TotalItems > 0);

            foreach (var ted in apiResponse.Data.Items)
            {
                Assert.Equal(statusEspecifico, ted.Status);
            }
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Status Sem TEDs Associados Retorna Lista Vazia"), Order(6)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorStatusSemTeds_RetornaListaVazia()
        {
            // Arrange
            var statusEspecifico = StatusEnum.Canceled;

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&status={statusEspecifico}");

            // Act
            var response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count == 0);
            Assert.True(apiResponse.Data.TotalItems == 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Cliente ID e Status Retorna Lista"), Order(7)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorClienteIdEStatus_RetornaLista()
        {
            // Arrange
            var statusEspecifico = StatusEnum.InProcess;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&status={statusEspecifico}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count > 0);
            Assert.True(apiResponse.Data.TotalItems > 0);
            foreach (var ted in apiResponse.Data.Items)
            {
                Assert.Equal(statusEspecifico, ted.Status);
            }
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Cliente ID e Status Sem TEDs Retorna Lista Vazia"), Order(8)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorClienteIdEStatusSemTeds_RetornaListaVazia()
        {
            // Arrange
            var statusEspecifico = StatusEnum.Canceled;

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&status={statusEspecifico}");

            // Act
            var response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count == 0);
            Assert.True(apiResponse.Data.TotalItems == 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Cliente ID e Data de Início Retorna Lista Não Vazia"), Order(9)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorClienteIdEDataInicio_RetornaListaNaoVazia()
        {
            // Arrange
            var dataInicio = DateTime.Now;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&startDate={dataInicio:yyyy-MM-dd}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count > 0);
            Assert.True(apiResponse.Data.TotalItems > 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Cliente ID e Data de Início Sem TEDs Retorna Lista Vazia"), Order(10)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorClienteIdEDataInicioSemTeds_RetornaListaVazia()
        {
            // Arrange
            var clienteId = 1001;
            var dataInicio = DateTime.Now.AddDays(-10);

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={clienteId}");
            urlBuilder.Append($"&startDate={dataInicio:yyyy-MM-dd}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count == 0);
            Assert.True(apiResponse.Data.TotalItems == 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Cliente ID, Data de Início e Fim Retorna Lista Não Vazia"), Order(11)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorClienteIdDataInicioEDataFim_RetornaListaNaoVazia()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-10);
            var dataFim = DateTime.Now;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&startDate={dataInicio:yyyy-MM-dd}");
            urlBuilder.Append($"&endDate={dataFim:yyyy-MM-dd}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count > 0);
            Assert.True(apiResponse.Data.TotalItems > 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Cliente ID, Data de Início e Fim Sem TEDs Retorna Lista Vazia"), Order(12)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPorClienteIdDataInicioEDataFim_RetornaListaVazia()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-10);
            var dataFim = DateTime.Now.AddDays(-5);

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&startDate={dataInicio:yyyy-MM-dd}");
            urlBuilder.Append($"&endDate={dataFim:yyyy-MM-dd}");

            // Act
            var response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count == 0);
            Assert.True(apiResponse.Data.TotalItems == 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Status, Cliente ID, Data de Início e Fim Retorna Lista Não Vazia"), Order(13)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPeloStatusClienteIdDataInicioEDataFim_RetornaListaNaoVazia()
        {
            // Arrange
            var statusEspecifico = StatusEnum.InProcess;
            var dataInicio = DateTime.Now.AddDays(-10);
            var dataFim = DateTime.Now;

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&startDate={dataInicio:yyyy-MM-dd}");
            urlBuilder.Append($"&endDate={dataFim:yyyy-MM-dd}");
            urlBuilder.Append($"&status={statusEspecifico}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count > 0);
            Assert.True(apiResponse.Data.TotalItems > 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada Filtrada por Status, Cliente ID, Data de Início e Fim Sem TEDs Retorna Lista Vazia"), Order(13)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginada_FiltradaPeloStatusClienteIdDataInicioEDataFim_RetornaListaVazia()
        {
            // Arrange
            var statusEspecifico = StatusEnum.Canceled;
            var dataInicio = DateTime.Now.AddDays(-100);
            var dataFim = DateTime.Now.AddDays(-50);

            var response = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
            response.EnsureSuccessStatusCode();

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?clientId={_tedDto.ClienteId}");
            urlBuilder.Append($"&startDate={dataInicio:yyyy-MM-dd}");
            urlBuilder.Append($"&endDate={dataFim:yyyy-MM-dd}");
            urlBuilder.Append($"&status={statusEspecifico}");

            // Act
            response = await _client.GetAsync(urlBuilder.ToString());

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            Assert.True(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.Data);
            Assert.True(apiResponse.Data.Items.Count == 0);
            Assert.True(apiResponse.Data.TotalItems == 0);
        }

        [Fact(DisplayName = "Obtem Lista TED Paginada pelo NumeroPagina e QuantidadeItensPagina e Retorna Lista Não Vazia"), Order(14)]
        [Trait("Categoria", "AdminObtemListaTedPaginada")]
        public async Task ObtemListaTedPaginadaPeloNumeroPaginaEQuantidadeItensPagina_RetornaListaNaoVazia()
        {
            // Arrange
            var numeroPagina = 1;
            var quantidadeItensPagina = 5;

            for (int i = 0; i < 100; i++)
            {
                var solicitaResponse = await _client.PostAsJsonAsync($"{_url}/cliente/solicita-ted", _tedDto);
                solicitaResponse.EnsureSuccessStatusCode();
            }

            var urlBuilder = new StringBuilder($"{_url}/admin/lista-ted");
            urlBuilder.Append($"?pageNumber={numeroPagina}");
            urlBuilder.Append($"&pageSize={quantidadeItensPagina}");


            // Act
            var response = await _client.GetAsync(urlBuilder.ToString());
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tedList = JsonSerializer.Deserialize<ApiResponse<PagedResponse<AdminTedResponseDto>>>(content);

            // Assert
            Assert.True(tedList.IsSuccess);
            Assert.True(tedList.Data.CurrentPage == numeroPagina);
            Assert.True(tedList.Data.Items.Count == quantidadeItensPagina);
            Assert.True(tedList.Data.PageSize == quantidadeItensPagina);
            Assert.True(tedList.Data.TotalItems == tedList.Data.TotalPages * quantidadeItensPagina);
        }
    }
}