using Moq;
using TED.API.Services;
using TED.API.Interfaces;
using TED.API.DTOs;
using TED.API.Exceptions;
using AutoMapper;
using TED.API.Entities;
using Xunit;

namespace TED.Tests.Admin.TesteUnidade;

public class AdminReprovaTedUnitTests
{
    private readonly Mock<IAdminTedRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISinacorTedService> _mockSinacorTedService;
    private readonly AdminTedService _service;

    public AdminReprovaTedUnitTests()
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

    [Fact(DisplayName = "Admin ReprovaTed Retorna Sucesso")]
    [Trait("Categoria", "AdminReprovaTed")]
    public async Task ReprovaTedRetornaSucesso()
    {
        // Arrange
        var clienteId = 1;
        var adminTedRequestDto = new AdminTedRequestDto
        {
            MotivoReprovacao = "motivo da reprovação"
        };

        _mockRepo.Setup(repo => repo.ObtemTedPeloIdAsync(clienteId))
                 .ReturnsAsync(new Ted());
        _mockRepo.Setup(repo => repo.ReprovaTedAsync(clienteId, adminTedRequestDto))
                .Verifiable();

        // Act
        await _service.ReprovaTedAsync(clienteId, adminTedRequestDto);

        // Assert
        _mockRepo.Verify(repo => repo.ReprovaTedAsync(clienteId, adminTedRequestDto), Times.Once);
    }


    [Fact(DisplayName = "Admin ReprovaTed Sem Ted para o ClienteId Informado Retorna Erro")]
    [Trait("Categoria", "AdminReprovaTed")]
    public async Task ReprovaTedSemTedParaOClienteIdInformadoRetornaException()
    {
        // Arrange
        var clienteId = 1;
        var adminTedRequestDto = new AdminTedRequestDto
        {
            MotivoReprovacao = "motivo da reprovação"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.ReprovaTedAsync(clienteId, new AdminTedRequestDto()));
        Assert.Equal("não existe Ted com o Id informado.", exception.Message);
    }


    [Fact(DisplayName = "Admin ReprovaTed Com Erro de Validação Retorna Erro")]
    [Trait("Categoria", "AdminReprovaTed")]
    public async Task ReprovaTedComErroValidacaoRetornaException()
    {
        // Arrange
        var clienteId = 0;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.ReprovaTedAsync(clienteId, new AdminTedRequestDto()));
    }
}