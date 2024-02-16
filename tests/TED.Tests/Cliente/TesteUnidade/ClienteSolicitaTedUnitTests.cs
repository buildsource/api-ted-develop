using Moq;
using TED.API.Services;
using TED.API.Interfaces;
using TED.API.DTOs;
using TED.API.Exceptions;
using TED.API.Entities;
using AutoMapper;
using Microsoft.Extensions.Options;
using TED.API.Enums;
using Xunit;

namespace TED.Tests.Cliente.TesteUnidade;

public class ClienteSolicitaTedUnitTests
{
    private readonly Mock<IClienteTedRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISinacorTedService> _mockSinacorTedService;
    private readonly ClienteTedService _service;
    private readonly Mock<IOptions<SinacorConfiguration>> _mockSinacorConfig;

    public ClienteSolicitaTedUnitTests()
    {
        _mockRepo = new Mock<IClienteTedRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockSinacorTedService = new Mock<ISinacorTedService>();
        _mockSinacorConfig = new Mock<IOptions<SinacorConfiguration>>();

        var sinacorConfig = new SinacorConfiguration
        {
            HorarioInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1),
            HorarioFim = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
        };

        _mockSinacorConfig.Setup(c => c.Value).Returns(sinacorConfig);

        _service = new ClienteTedService(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockSinacorTedService.Object,
            _mockSinacorConfig.Object
        );
    }

    [Fact(DisplayName = "Cliente SolicitaTed Dentro do Limite Retorna Sucesso")]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitaTedDentroDoLimiteRetornaSucesso()
    {
        // Arrange
        var sinacorConfirmacaoId = "F10";
        var tedDto = new ClienteTedRequestDto
        {
            ClienteId = 1,
            DataAgendamento = DateTime.Now,
            ValorSolicitado = 1000.00,
            NumeroAgencia = "0001",
            NumeroConta = "26054",
            DigitoConta = "0",
            NumeroBanco = "83"
        };


        var ted = new Ted
        {
            ClienteId = 1,
            DataAgendamento = DateTime.Now,
            ValorSolicitado = 1000.00,
            NumeroAgencia = "0001",
            NumeroConta = "26054",
            DigitoConta = "0",
            NumeroBanco = "83"
        };

        var tedResponseDto = new ClienteTedResponseDto
        {
            SinacorConfirmacaoId = sinacorConfirmacaoId,
            Status = StatusEnum.Approved
        };

        _mockRepo.Setup(repo => repo.VerificaLimiteTedDoDiaAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Ted>
                {
                    {
                        ted
                    }
                });
        _mockSinacorTedService.Setup(repo => repo.ClienteEnviaTedParaSinacorAsync(It.IsAny<SinacorTedRequestDto>()))
                .ReturnsAsync(sinacorConfirmacaoId);
        _mockRepo.Setup(repo => repo.SolicitaTedAsync(It.IsAny<Ted>()))
                .ReturnsAsync(new Ted());
        _mockMapper.Setup(m => m.Map<Ted>(It.IsAny<ClienteTedRequestDto>()))
                .Returns(ted);
        _mockMapper.Setup(m => m.Map<ClienteTedResponseDto>(It.IsAny<Ted>()))
                .Returns(tedResponseDto);
        _mockRepo.Setup(repo => repo.ObtemLimiteTedAsync())
                .ReturnsAsync(new LimiteTed()
                {
                    QuantidadeMaximaDia = 10,
                    ValorMaximoDia = 10000,
                    ValorMaximoPorSaque = 5000,
                });
        // Act
        var result = await _service.SolicitaTedAsync(tedDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ClienteTedResponseDto>(result);
        Assert.Equal(sinacorConfirmacaoId, result.SinacorConfirmacaoId);
        Assert.Equal(StatusEnum.Approved, result.Status);
    }


    [Fact(DisplayName = "Cliente SolicitaTed Dentro do Limite Com Erro de Validação Retorna Erro")]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitaTedDentroDoLimiteComErroValidacaoRetornaException()
    {
        // Arrange
        var tedDto = new ClienteTedRequestDto {
            ClienteId = -1,
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.SolicitaTedAsync(tedDto));
    }

    [Fact(DisplayName = "Cliente SolicitaTed Acima da Quantidade Maxima Dia Retorna Erro")]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitaTedAcimaDaQuantidadeMaximaDiaRetornaException()
    {
        // Arrange
        var tedDto = new ClienteTedRequestDto();
        var tedsExistentes = new List<Ted>
        {
            new Ted(), new Ted(), new Ted() // Simula que já atingiu o limite diário
        };

        _mockRepo.Setup(repo => repo.VerificaLimiteTedDoDiaAsync(It.IsAny<int>()))
                .ReturnsAsync(tedsExistentes);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.SolicitaTedAsync(tedDto));
    }

    [Fact(DisplayName = "Cliente SolicitaTed Acima do Valor Maximo Dia Retorna Erro")]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitaTedAcimaDoValorMaximoDiaRetornaException()
    {
        // Arrange
        var tedDto = new ClienteTedRequestDto { ValorSolicitado = 8000 };
        var tedsExistentes = new List<Ted> { new Ted { ValorSolicitado = 3000 } }; // Totalizando 11000, acima do limite diário

        _mockRepo.Setup(repo => repo.VerificaLimiteTedDoDiaAsync(It.IsAny<int>()))
                .ReturnsAsync(tedsExistentes);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.SolicitaTedAsync(tedDto));
    }

    [Fact(DisplayName = "Cliente SolicitaTed Acima do Valor por Saque Retorna Erro")]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitaTedAcimaDoValorMaximoPorSaqueRetornaException()
    {
        // Arrange
        var tedDto = new ClienteTedRequestDto { ValorSolicitado = 6000 }; // Acima do limite por saque

        _mockRepo.Setup(repo => repo.VerificaLimiteTedDoDiaAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Ted>());

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.SolicitaTedAsync(tedDto));
    }

    [Fact(DisplayName = "Cliente SolicitaTed Agendado Retorna Sucesso")]
    [Trait("Categoria", "ClienteSolicitaTed")]
    public async Task SolicitaTedAgendadoRetornaSucesso()
    {
        // Arrange
        var tedDto = new ClienteTedRequestDto
        {
            ClienteId = new Random().Next(1,998),
            DataAgendamento = DateTime.Now.AddDays(1),
            ValorSolicitado = 1000.00,
            NumeroAgencia = "0001",
            NumeroConta = "26054",
            DigitoConta = "0",
            NumeroBanco = "83"
        };


        var ted = new Ted
        {
            ClienteId = tedDto.ClienteId,
            DataAgendamento = tedDto.DataAgendamento,
            ValorSolicitado = tedDto.ValorSolicitado,
            NumeroAgencia = tedDto.NumeroAgencia,
            NumeroConta = tedDto.NumeroConta,
            NumeroBanco = tedDto.NumeroBanco,
            Status = StatusEnum.InProcess
        };

        var tedResponseDto = new ClienteTedResponseDto
        {
            SinacorConfirmacaoId = string.Empty,
            Status = StatusEnum.InProcess
        };

        _mockRepo.Setup(repo => repo.VerificaLimiteTedDoDiaAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Ted>
                {
                    {
                        ted
                    }
                });
        _mockSinacorTedService.Setup(repo => repo.ClienteEnviaTedParaSinacorAsync(It.IsAny<SinacorTedRequestDto>()))
                .ReturnsAsync(string.Empty);
        _mockRepo.Setup(repo => repo.SolicitaTedAsync(It.IsAny<Ted>()))
                .ReturnsAsync(new Ted());
        _mockMapper.Setup(m => m.Map<Ted>(It.IsAny<ClienteTedRequestDto>()))
                .Returns(ted);
        _mockMapper.Setup(m => m.Map<ClienteTedResponseDto>(It.IsAny<Ted>()))
                .Returns(tedResponseDto);
        _mockRepo.Setup(repo => repo.ObtemLimiteTedAsync())
                .ReturnsAsync(new LimiteTed()
                {
                    QuantidadeMaximaDia = 10,
                    ValorMaximoDia = 10000,
                    ValorMaximoPorSaque = 5000,
                });

        // Act
        var result = await _service.SolicitaTedAsync(tedDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ClienteTedResponseDto>(result);
        Assert.Equal(string.Empty, result.SinacorConfirmacaoId);
        Assert.Equal(StatusEnum.InProcess, result.Status);
    }
}
