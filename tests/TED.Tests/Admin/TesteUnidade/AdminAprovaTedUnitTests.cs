using Moq;
using TED.API.Services;
using TED.API.Interfaces;
using TED.API.DTOs;
using TED.API.Exceptions;
using AutoMapper;
using TED.API.Entities;
using TED.API.Parameters;
using Xunit;


namespace TED.Tests.Admin.TesteUnidade;

public class AdminAprovaTedUnitTests
{
    private readonly Mock<IAdminTedRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISinacorTedService> _mockSinacorTedService;
    private readonly AdminTedService _service;

    public AdminAprovaTedUnitTests()
    {
        _mockRepo = new Mock<IAdminTedRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockSinacorTedService = new Mock<ISinacorTedService>();

        _service = new AdminTedService(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockSinacorTedService.Object
        );
    }

    [Fact(DisplayName = "Admin AprovaTed Retorna Sucesso")]
    [Trait("Categoria", "AdminAprovaTed")]
    public async Task AprovaTedRetornaSucesso()
    {
        // Arrange
        var clienteId = 1;
        var sinacorConfirmacaoId = "F10";

        var queryParameters = new AdminTedQueryParameters()
        {
            ClienteId = clienteId,
        };

        _mockRepo.Setup(repo => repo.ObtemTedPeloIdAsync(clienteId))
                 .ReturnsAsync(new Ted
                 {
                     ClienteId = new Random().Next(1, 998),
                     DataAgendamento = DateTime.Now.AddDays(1),
                     ValorSolicitado = 100000,
                     NumeroAgencia = "1234",
                     NumeroConta = "1234567",
                     NumeroBanco = "123"
                 });
        _mockSinacorTedService.Setup(repo => repo.ClienteEnviaTedParaSinacorAsync(It.IsAny<SinacorTedRequestDto>()))
                .ReturnsAsync(sinacorConfirmacaoId);
        _mockRepo.Setup(repo => repo.AprovaTedAsync(clienteId, sinacorConfirmacaoId))
                .Verifiable();

        // Act
        await _service.AprovaTedAsync(clienteId);

        // Assert
        _mockRepo.Verify(repo => repo.AprovaTedAsync(clienteId, sinacorConfirmacaoId), Times.Once);
    }


    [Fact(DisplayName = "Admin AprovaTed Sem Ted para o CLienteId Informado Retorna Erro")]
    [Trait("Categoria", "AdminAprovaTed")]
    public async Task AprovaTedSemTedParaOClienteIdInformadoRetornaException()
    {
        // Arrange
        var clienteId = 1;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.AprovaTedAsync(clienteId));
        Assert.Equal("não existe Ted com o Id informado.", exception.Message);
    }


    [Fact(DisplayName = "Admin AprovaTed Com Erro de Validação Retorna Erro")]
    [Trait("Categoria", "AdminAprovaTed")]
    public async Task AprovaTedComErroValidacaoRetornaException()
    {
        // Arrange
        var clienteId = 0;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.AprovaTedAsync(clienteId));
    }
}