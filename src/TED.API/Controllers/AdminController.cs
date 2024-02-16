using Microsoft.AspNetCore.Mvc;
using TED.API.DTOs;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Parameters;
using TED.API.Response;

namespace TED.API.Controllers;

/// <summary>
/// Controlador responsável por operações administrativas relacionadas a TEDs.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IAdminTedService _service;

    public AdminController(
        ILogger<AdminController> logger,
        IAdminTedService service
    )
    {
        _logger = logger;
        _service = service;
    }


    /// <summary>
    /// Obtém uma lista paginada de TEDs com base nos parâmetros de consulta fornecidos.
    /// Esta operação filtra e retorna TEDs conforme os critérios especificados nos parâmetros de consulta.
    /// A resposta inclui dados paginados, facilitando a manipulação de grandes conjuntos de dados.
    /// </summary>
    /// <param name="queryParameters">Parâmetros de consulta para filtrar e paginar a lista de TEDs. 
    /// Inclui opções para especificar o número da página e o tamanho da página, além de outros critérios de filtragem.</param>
    /// <returns>Uma resposta API contendo a lista paginada de TEDs junto com detalhes de paginação como o número total de itens e páginas.</returns>
    /// <response code="200">Retorna a lista paginada de TEDs, com informações sobre a contagem total de itens e o número total de páginas, baseadas nos critérios de consulta fornecidos.</response>
    /// <response code="400">Retorna um erro se os parâmetros de consulta são inválidos ou se ocorre um erro durante a obtenção dos dados.</response>

    [HttpGet("lista-ted")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<AdminTedResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtemTedAsync([FromQuery] AdminTedQueryParameters queryParameters)
    {
        try
        {
            var teds = await _service.ObtemListaTedAsync(queryParameters);

            return Ok(new ApiResponse<PagedResponse<AdminTedResponseDto>>(teds, "Ted listado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao listar o Ted", new List<string> { ex.Message }));
        }
    }


    /// <summary>
    /// Aprova um TED pelo Id.
    /// </summary>
    /// <param name="id">ID do TED a ser aprovado.</param>
    /// <returns>Confirmação da aprovação do TED.</returns>
    /// <response code="200">Confirma a aprovação do TED.</response>
    /// <response code="400">Se o ID do TED é inválido ou ocorre um erro.</response>

    [HttpPut("aprova-ted/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AdminTedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AprovaTedAsync(int id)
    {
        try
        {
            await _service.AprovaTedAsync(id);

            return Ok(new ApiResponse<AdminTedResponseDto>(null, "Ted aprovado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao aprovar o Ted", new List<string> { ex.Message }));
        }
    }


    /// <summary>
    /// Reprova um TED pelo Id.
    /// </summary>
    /// <param name="adminTedRequestDto">Motivo da reprovação do TED.</param>
    /// <param name="id">ID do TED a ser reprovado.</param>
    /// <returns>Confirmação da reprovação do TED.</returns>
    /// <response code="200">Confirma a reprovação do TED.</response>
    /// <response code="400">Se o ID do TED é inválido, os dados são inválidos ou ocorre um erro.</response>

    [HttpPut("reprova-ted/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AdminTedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReprovaTedAsync([FromBody] AdminTedRequestDto? adminTedRequestDto, int id)
    {
        try
        {
            await _service.ReprovaTedAsync(id, adminTedRequestDto);

            return Ok(new ApiResponse<AdminTedResponseDto>(null, "Ted reprovado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao reprovar o Ted", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Atualiza o limite de TED.
    /// </summary>
    /// <param name="limiteTedRequestDto">Dados para atualização do limite de TED.</param>
    /// <returns>Confirmação da atualização do limite de TED.</returns>
    /// <response code="200">Confirma a atualização do limite de TED.</response>
    /// <response code="400">Se os dados são inválidos ou ocorre um erro.</response>
    [HttpPut("atualiza-limite-ted")]
    [ProducesResponseType(typeof(ApiResponse<LimiteTedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizaLimiteTedAsync([FromBody] LimiteTedRequestDto limiteTedRequestDto)
    {
        try
        {
            await _service.AtualizaLimiteTedAsync(limiteTedRequestDto);

            return Ok(new ApiResponse<AdminTedResponseDto>(null, "Limite do Ted atualizado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao atualizar o limite do Ted", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Obtém o limite atual para a realização de TEDs.
    /// </summary>
    /// <returns>Uma resposta API contendo os detalhes do limite atual para TEDs, incluindo o valor do limite.</returns>
    /// <response code="200">Retorna o limite atual para TEDs, facilitando o monitoramento e ajustes necessários por parte dos administradores.</response>
    /// <response code="400">Retorna um erro se ocorre um problema ao tentar obter o limite de TED, incluindo falhas de sistema ou dados incompletos.</response>
    [HttpGet("obtem-limite-ted")]
    [ProducesResponseType(typeof(ApiResponse<LimiteTedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtemLimiteTedAsync()
    {
        try
        {
            var limiteTedResponseDto = await _service.ObtemLimiteTedAsync();

            return Ok(new ApiResponse<LimiteTedResponseDto>(limiteTedResponseDto, "Limite do Ted listado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao listar o limite do Ted", new List<string> { ex.Message }));
        }
    }
}