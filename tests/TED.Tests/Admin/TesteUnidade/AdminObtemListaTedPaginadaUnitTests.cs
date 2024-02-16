using AutoMapper;
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

namespace TED.Tests.Admin.TesteUnidade;

public class AdminObtemListaTedPaginadaUnitTests
{
    private readonly Mock<IAdminTedRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISinacorTedService> _mockSinacorTedService;
    private readonly AdminTedService _service;

    public AdminObtemListaTedPaginadaUnitTests()
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

    [Fact(DisplayName = "Admin ObtemListaTed Retorna Lista Preenchida")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ReturnsList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters();
        var tedList = new List<Ted>
        {
            new Ted
            {
                ClienteId = 1,
                DataAgendamento = DateTime.Now,
                ValorSolicitado = 1000.00,
                NumeroAgencia = "1234",
                NumeroConta = "567890",
                NumeroBanco = "001",
                Status = StatusEnum.InProcess
            },
        };

        var tedResponseList = tedList.Select(ted => new AdminTedResponseDto()).ToList();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((tedList, tedList.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(tedResponseList);

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == tedList.Count);
        Assert.True(result.TotalItems == tedList.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Inválido Gera Exceção")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_InvalidClienteId_ThrowsValidationException()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters()
        {
            ClienteId = -1, // Invalid ID
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.ObtemListaTedAsync(queryParameters));

        // Verificar se a exceção contém as mensagens de erro esperadas
        Assert.Contains("ClientId deve ser maior que 0", exception.Errors);
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Válido Retorna Lista com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteId_ReturnsList()
    {
        // Arrange
        var clienteId = 1; // Valid ID
        var queryParameters = new AdminTedQueryParameters()
        {
            ClienteId = clienteId
        };
        var tedList = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = DateTime.Now,
                ValorSolicitado = 1000.00,
                NumeroAgencia = "1234",
                NumeroConta = "567890",
                NumeroBanco = "001",
                Status = StatusEnum.InProcess
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((tedList, tedList.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(tedList.Select(t => new AdminTedResponseDto()
                   {
                       ClienteId = t.ClienteId,
                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == tedList.Count);
        Assert.True(result.TotalItems == tedList.Count);
        Assert.All(result.Items, item => Assert.Equal(clienteId, item.ClienteId));
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Válido Retorna Lista Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters()
        {
            ClienteId = 2, // Valid ID
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed Filtrado pelo Status Retorna Lista com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_Status_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var status = StatusEnum.InProcess; // Um status específico
        var queryParameters = new AdminTedQueryParameters
        {
            Status = status
        };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                Status = status,
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto()).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.Equal(status, item.Status));
    }

    [Fact(DisplayName = "Admin ObtemListaTed Filtrada pelo Status Sem TEDs Retorna Lista Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_StatusNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters()
        {
            Status = StatusEnum.Disapproved
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Válido e Filtrada por Status Retorna Lista com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStatus_ReturnsList()
    {
        // Arrange
        var status = StatusEnum.Canceled; // Um status específico
        var clienteId = 1; // Um ID válido
        var queryParameters = new AdminTedQueryParameters
        {
            Status = status,
            ClienteId = clienteId
        };
        var listaTed = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = DateTime.Now,
                ValorSolicitado = 1000.00,
                NumeroAgencia = "1234",
                NumeroConta = "567890",
                NumeroBanco = "001",
                Status = StatusEnum.Canceled
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                .Returns(listaTed.Select(t => new AdminTedResponseDto
                 {
                    ClienteId = t.ClienteId,
                    Status = t.Status

                 }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
        Assert.All(result.Items, item => Assert.Equal(clienteId, item.ClienteId));
        Assert.All(result.Items, item => Assert.Equal(status, item.Status));
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Válido e Status Sem TEDs Retorna Lista Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStatusWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters { 
            Status = StatusEnum.Canceled, // Um status específico sem TEDs associados
            ClienteId = 1 // Um ID válido
        };
        var emptyTedlist = new List<Ted>(); // Lista vazia de TEDs

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((emptyTedlist, emptyTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == emptyTedlist.Count);
        Assert.True(result.TotalItems == emptyTedlist.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed Filtrada por Data Início Específica Retorna Lista Não Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_Date_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de início específica
        var queryParameters = new AdminTedQueryParameters
        {
            DataInicio = startDate,
            DataFim = endDate,
        };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                DataAgendamento = startDate.AddDays(1), // Data dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto
                   {
                       DataAgendamento = t.DataAgendamento,

                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.True(item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Admin ObtemListaTed Filtrada pela Data Início Sem TEDs Associados Retorna Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_StartDateWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters
        {
            DataInicio = new DateTime(2023, 1, 1), // Uma data de início específica sem TEDs associados
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Válido e Data Início Específica Retorna Lista Não Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStartDate_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var queryParameters = new AdminTedQueryParameters { 
            DataInicio = startDate,
            ClienteId = clienteId,
        };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = startDate.AddDays(1), // Data dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto
                   {
                       ClienteId = clienteId,
                       DataAgendamento = t.DataAgendamento,

                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.Equal(clienteId, item.ClienteId));
        Assert.All(result.Items, item => Assert.True(item.DataAgendamento >= startDate));
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId e Data Início Sem TEDs Associados Retorna Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndStartDateWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters { 
            DataInicio = new DateTime(2023, 1, 1), // Uma data de início específica sem TEDs associados
            ClienteId = 1 // Um ID válido
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed Intervalo de Datas Retorna com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_DateRange_ReturnsList()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica,
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new AdminTedQueryParameters
        {
            DataInicio = startDate,
            DataFim = endDate
        };

        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                DataAgendamento = startDate.AddDays(5), // Data e status dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto
                   {
                       DataAgendamento = t.DataAgendamento,
                   }).ToList());
        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.True(item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Admin ObtemListaTed Intervalo de Datas Sem TEDs Retorna Vazia")]//n
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_DateRangeWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters
        {
            DataInicio = new DateTime(2023, 1, 1), // Uma data de início específica, 
            DataFim = new DateTime(2023, 1, 31), // Uma data de fim específica, 
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());
        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }



    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId Válido e Intervalo de Datas Retorna Lista com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndDateRange_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new AdminTedQueryParameters { 
            DataInicio = startDate, 
            DataFim = endDate,
            ClienteId = clienteId
        };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                DataAgendamento = startDate.AddDays(5), // Data dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto
                   {
                       ClienteId = t.ClienteId,
                       DataAgendamento = t.DataAgendamento
                   }).ToList());
        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.Equal(clienteId, item.ClienteId));
        Assert.All(result.Items, item => Assert.True(item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId e Intervalo de Datas Sem TEDs Retorna Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdAndDateRangeWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters { 
            DataInicio = new DateTime(2023, 1, 1), // Uma data de início específica, 
            DataFim = new DateTime(2023, 1, 31), // Uma data de fim específica
            ClienteId = 1 // Um ID válido
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed Filtrada pelo Status Aprovado e Intervalo de Datas Retorna Lista com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_StatusAndDateRange_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var status = StatusEnum.Approved; // Um status específico
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new AdminTedQueryParameters
        {
            Status = status,
            DataInicio = startDate,
            DataFim = endDate,
        };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                Status = status,
                DataAgendamento = startDate.AddDays(5), // Data e status dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto
                   {
                       Status = t.Status,
                       DataAgendamento = t.DataAgendamento,
                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.True(item.Status == status && item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Admin ObtemListaTed Filtrada pelo Status Aprovado e Intervalo de Datas Sem TEDs Retorna Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_StatusAndDateRangeWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters
        {
            Status = StatusEnum.Approved, // Um status específico sem TEDs associados
            DataInicio = new DateTime(2023, 1, 1), // Uma data de início específica, 
            DataFim = new DateTime(2023, 1, 31), // Uma data de fim específica, 
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());
        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId e Filtrada pelo Status e Intervalo de Datas Retorna Lista com Itens")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdStatusAndDateRange_ReturnsNonEmptyFilteredList()
    {
        // Arrange
        var clienteId = 1; // Um ID válido
        var status = StatusEnum.Approved; // Um status específico
        var startDate = new DateTime(2023, 1, 1); // Uma data de início específica
        var endDate = new DateTime(2023, 1, 31); // Uma data de fim específica
        var queryParameters = new AdminTedQueryParameters { 
            Status = status, 
            DataInicio = startDate, 
            DataFim = endDate,
            ClienteId = clienteId
        };
        var filteredTedlist = new List<Ted>
        {
            new Ted
            {
                ClienteId = clienteId,
                Status = status,
                DataAgendamento = startDate.AddDays(5), // Data e status dentro do intervalo desejado
            },
        };

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((filteredTedlist, filteredTedlist.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(filteredTedlist.Select(t => new AdminTedResponseDto
                   {
                       ClienteId = t.ClienteId,
                       Status = t.Status,
                       DataAgendamento = t.DataAgendamento,
                   }).ToList());

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == filteredTedlist.Count);
        Assert.True(result.TotalItems == filteredTedlist.Count);
        Assert.All(result.Items, item => Assert.Equal(clienteId, item.ClienteId));
        Assert.All(result.Items, item => Assert.True(item.Status == status && item.DataAgendamento >= startDate && item.DataAgendamento <= endDate));
    }

    [Fact(DisplayName = "Admin ObtemListaTed com ClienteId e Filtrada pelo Status Aprovado e Intervalo de Datas Sem TEDs Retorna Vazia")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedAsync_ValidClienteIdStatusAndDateRangeWithNoTeds_ReturnsEmptyList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters { 
            Status = StatusEnum.Approved, // Um status específico sem TEDs associados
            DataInicio = new DateTime(2023, 1, 1), // Uma data de início específica, 
            DataFim = new DateTime(2023, 1, 31), // Uma data de fim específica, 
            ClienteId = 1
        };
        var listaTed = new List<Ted>();

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(queryParameters))
                 .ReturnsAsync((listaTed, listaTed.Count));
        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns(new List<AdminTedResponseDto>());
        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.True(result.Items.Count == listaTed.Count);
        Assert.True(result.TotalItems == listaTed.Count);
    }


    [Fact(DisplayName = "Admin ObtemListaTed Paginada Retorna Lista Preenchida")]
    [Trait("Categoria", "AdminObtemListaTed")]
    public async Task ObtemListaTedPaginadaAsync_ReturnsList()
    {
        // Arrange
        var queryParameters = new AdminTedQueryParameters()
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
                ClienteId = random.Next(1, 998),
                DataAgendamento = DateTime.Now,
                ValorSolicitado = 1000.00,
                NumeroAgencia = "1234",
                NumeroConta = "567890",
                NumeroBanco = "001",
                Status = StatusEnum.InProcess
            });
        }

        _mockRepo.Setup(repo => repo.ObtemListaTedAsync(It.IsAny<AdminTedQueryParameters>()))
                 .ReturnsAsync((tedList, tedList.Count));


        _mockMapper.Setup(mapper => mapper.Map<List<AdminTedResponseDto>>(It.IsAny<List<Ted>>()))
                   .Returns((List<Ted> source) =>
                   {
                       var pagedItems = source.Skip((queryParameters.NumeroPagina - 1) * queryParameters.QuantidadeItensPagina)
                                        .Take(queryParameters.QuantidadeItensPagina)
                                        .Select(t => new AdminTedResponseDto
                                        {
                                            ClienteId = t.ClienteId
                                        })
                                        .ToList();
                       return pagedItems;
                   });

        // Act
        var result = await _service.ObtemListaTedAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResponse<AdminTedResponseDto>>(result);
        Assert.Equal(queryParameters.QuantidadeItensPagina, result.Items.Count);
        Assert.Equal(queryParameters.NumeroPagina, result.CurrentPage);
        Assert.Equal((int)Math.Ceiling((double)tedList.Count / queryParameters.QuantidadeItensPagina), result.TotalPages);
    }
}
