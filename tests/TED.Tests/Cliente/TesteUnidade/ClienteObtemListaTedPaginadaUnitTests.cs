using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using TED.API.DTOs;
using TED.API.Entities;
using TED.API.Enums;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Parameters;
using TED.API.Response;
using TED.API.Services;
using Xunit;

namespace TED.Tests.Cliente.TesteUnidade;

public class ClienteObtemListaTedPaginadaUnitTests
{
    private readonly Mock<IClienteTedRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISinacorTedService> _mockSinacorTedService;
    private readonly ClienteTedService _service;
    private readonly Mock<IOptions<SinacorConfiguration>> _mockSinacorConfig;

    public ClienteObtemListaTedPaginadaUnitTests()
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

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Válido Retorna Lista Preenchida")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteId_ReturnsList()
    {
        // Arrange
        var clienteId = 1;
        var queryParameters = new ClienteTedQueryParameters();
        var listaTed = new List<Ted>
        {
            new Ted
            {
                ClienteId = 1,
                DataAgendamento = DateTime.Now.AddDays(1),
                ValorSolicitado = 1000.00,
                NumeroAgencia = "1234",
                NumeroConta = "567890",
                NumeroBanco = "001",
                Status = StatusEnum.InProcess
            },
        };


        var tedResponseList = listaTed.Select(ted => new ClienteTedResponseDto()).ToList();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(tedResponseList);

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.Equal(listaTed.Count, result.Items.Count);
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Inválido Gera Exceção")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_InvalidClienteId_ThrowsValidationException()
    {
        // Arrange
        var clienteId = -1; // Invalid ID
        var queryParameters = new ClienteTedQueryParameters();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.ObtemListaTedAsync(clienteId, queryParameters));

        // Verificar se a exceção contém as mensagens de erro esperadas
        Assert.Contains("O ClienteId não é válido", exception.Errors);
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Válido Retorna Lista Vazia")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var clienteId = 2; // Um ID válido, mas sem TEDs associados
        var queryParameters = new ClienteTedQueryParameters();
        var emptyTedlist = new List<Ted>(); // Lista vazia de TEDs

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((emptyTedlist, emptyTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<ClienteTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == 0);
        Assert.True(result.TotalItems == 0);
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Válido e Status InProcess Retorna Lista com Itens")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStatus_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var status = StatusEnum.InProcess; // Um status específico
        var queryParameters = new ClienteTedQueryParameters { Status = status };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                Status = status,
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new ClienteTedResponseDto()).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.Equal(status, item.Status));
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Válido e Status Sem TEDs Retorna Lista Vazia")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStatusWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var status = StatusEnum.Canceled; // Um status específico sem TEDs associados
        var queryParameters = new ClienteTedQueryParameters { Status = status };
        var emptyTedlist = new List<Ted>(); // Lista vazia de TEDs

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((emptyTedlist, emptyTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<ClienteTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == emptyTedlist.Count);
        Assert.True(result.TotalItems == emptyTedlist.Count);
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Válido e Data Início Específica Retorna Lista Não Vazia")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStartDate_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var queryParameters = new ClienteTedQueryParameters { DataInicio = startDate };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = startDate.AddDays(1), // Data dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new ClienteTedResponseDto
                   {
                       ClienteId = clienteId,
                       DataAgendamento = t.DataAgendamento,
                       
                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.True(item.DataAgendamento >= startDate));
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId e Data Início Sem TEDs Associados Retorna Vazia")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStartDateWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica sem TEDs associados
        var queryParameters = new ClienteTedQueryParameters { DataInicio = startDate };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                 .Returns(new List<ClienteTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId Válido e Intervalo de Datas Retorna Lista Preenchida")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndDateRange_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new ClienteTedQueryParameters { DataInicio = startDate, DataFim = endDate };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = startDate.AddDays(5), // Data dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new ClienteTedResponseDto
                   {
                       DataAgendamento = t.DataAgendamento
                   }).ToList());
        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.True(item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId e Intervalo de Datas Sem TEDs Retorna Vazia")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndDateRangeWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new ClienteTedQueryParameters { DataInicio = startDate, DataFim = endDate };
        var emptyTedlist = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((emptyTedlist, emptyTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<ClienteTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == emptyTedlist.Count);
        Assert.True(result.TotalItems == emptyTedlist.Count);
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId, Status Aprovado e Intervalo de Datas Retorna Lista com Itens")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdStatusAndDateRange_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var status = StatusEnum.Approved; // Um status específico
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new ClienteTedQueryParameters { Status = status, DataInicio = startDate, DataFim = endDate };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                Status = status,
                DataAgendamento = startDate.AddDays(5), // Data e status dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new ClienteTedResponseDto
                   {
                       ClienteId = t.ClienteId,
                       Status = t.Status,
                       DataAgendamento = t.DataAgendamento,
                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.True(item.Status == status && item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Cliente ObtemListaTed com ClienteId, Status Aprovado e Intervalo de Datas Sem TEDs Retorna Vazia")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdStatusAndDateRangeWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var status = StatusEnum.Approved; // Um status específico sem TEDs associados
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new ClienteTedQueryParameters { Status = status, DataInicio = startDate, DataFim = endDate };
        var emptyTedlist = new List<Ted>(); 

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((emptyTedlist, emptyTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<ClienteTedResponseDto>());
        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.True(result.Items.Count == emptyTedlist.Count);
        Assert.True(result.TotalItems == emptyTedlist.Count);
    }


    [Fact(DisplayName = "Cliente ObtemListaTed Paginada Retorna Lista Preenchida")]
    [Trait("Categoria", "ClienteObtemListaTed")]
    public async Task ObtemListaTedPaginadaAsync_ReturnsList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var queryParameters = new ClienteTedQueryParameters()
        {
            NumeroPagina = 1,
            QuantidadeItensPagina = 5
        };


        var tedList = new List<Ted>();
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            tedList.Add(new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = DateTime.Now,
                ValorSolicitado = 1000.00,
                NumeroAgencia = "1234",
                NumeroConta = "567890",
                NumeroBanco = "001",
                Status = StatusEnum.InProcess
            });
        }

        var tedResponseList = tedList.Select(ted => new ClienteTedResponseDto()).ToList();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(clienteId, queryParameters))
                 .ReturnsAsync((tedList, tedList.Count));


        _mockMapper.Setup(mapper => mapper.Map<List<ClienteTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns((List<Ted> source) =>
                   {
                       var pagedItems = source.Skip((queryParameters.NumeroPagina - 1) * queryParameters.QuantidadeItensPagina)
                                        .Take(queryParameters.QuantidadeItensPagina)
                                        .Select(t => new ClienteTedResponseDto
                                        {
                                            ClienteId = t.ClienteId
                                        })
                                        .ToList();
                       return pagedItems;
                   });

        // Act
        var result = await _service.ObtemListaTedAsync(clienteId, queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<ClienteTedResponseDto>>(result);
        Assert.Equal(queryParameters.QuantidadeItensPagina, result.Items.Count);
        Assert.Equal(queryParameters.NumeroPagina, result.CurrentPage);
        Assert.Equal((int)Math.Ceiling((double)tedList.Count / queryParameters.QuantidadeItensPagina), result.TotalPages);
    }
}
