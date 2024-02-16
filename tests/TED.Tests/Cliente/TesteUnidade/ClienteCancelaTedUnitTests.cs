using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using TED.API.Configuration;
using TED.API.Entities;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Services;
using Xunit;

namespace TED.Tests.Cliente.TesteUnidade;

public class ClienteCancelaTedUnitTests
{
    private readonly Mock<IClienteTedRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISinacorTedService> _mockSinacorTedService;
    private readonly ClienteTedService _service;
    private readonly Mock<IOptions<SinacorConfiguration>> _mockSinacorConfig;

    public ClienteCancelaTedUnitTests()
    {
        _mockRepo = new Mock<IClienteTedRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockSinacorTedService = new Mock<ISinacorTedService>();
        _mockSinacorConfig = new Mock<IOptions<SinacorConfiguration>>();

        _service = new ClienteTedService(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockSinacorTedService.Object,
            _mockSinacorConfig.Object
        );
    }

    [Fact(DisplayName = "Cliente CancelaTed com ClienteId Válido Retorna Sucesso")]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTed_PeloClienteIdRetornaSucesso()
    {
        // Arrange
        int validId = 1; // ID válido
        _mockRepo.Setup(repo => repo.CancelaTedAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);
        _mockRepo.Setup(repo => repo.ObtemTedPeloIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(new Ted()));

        // Act
        await _service.CancelaTedAsync(validId);


        // Assert
        _mockRepo.Verify(repo => repo.CancelaTedAsync(validId), Times.Once);
    }

    [Fact(DisplayName = "Cliente CancelaTed com ClienteId Inválido Retorna Exception")]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTed_PeloClienteIdComErroValidacaoRetornaException()
    {
        // Arrange
        int invalidId = -1; // ID inválido

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () => await _service.CancelaTedAsync(invalidId));
    }

    [Fact(DisplayName = "Cliente ReprovaTed Sem Ted para o ClienteId Informado Retorna Erro")]
    [Trait("Categoria", "ClienteCancelaTed")]
    public async Task CancelaTedSemTedParaOClienteIdInformadoRetornaException()
    {
        // Arrange
        var clienteId = 1;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.CancelaTedAsync(clienteId));
        Assert.Equal("não existe Ted com o Id informado.", exception.Message);
    }
}
