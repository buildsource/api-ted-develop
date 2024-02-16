using Microsoft.AspNetCore.Mvc;
using TED.API.DTOs;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Parameters;
using TED.API.Response;

namespace TED.API.Controllers;

/// <summary>
/// Controlador respons�vel por opera��es administrativas relacionadas a TEDs.
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
    /// Obt�m uma lista paginada de TEDs com base nos par�metros de consulta fornecidos.
    /// Esta opera��o filtra e retorna TEDs conforme os crit�rios especificados nos par�metros de consulta.
    /// A resposta inclui dados paginados, facilitando a manipula��o de grandes conjuntos de dados.
    /// </summary>
    /// <param name="queryParameters">Par�metros de consulta para filtrar e paginar a lista de TEDs. 
    /// Inclui op��es para especificar o n�mero da p�gina e o tamanho da p�gina, al�m de outros crit�rios de filtragem.</param>
    /// <returns>Uma resposta API contendo a lista paginada de TEDs junto com detalhes de pagina��o como o n�mero total de itens e p�ginas.</returns>
    /// <response code="200">Retorna a lista paginada de TEDs, com informa��es sobre a contagem total de itens e o n�mero total de p�ginas, baseadas nos crit�rios de consulta fornecidos.</response>
    /// <response code="400">Retorna um erro se os par�metros de consulta s�o inv�lidos ou se ocorre um erro durante a obten��o dos dados.</response>

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
    /// <returns>Confirma��o da aprova��o do TED.</returns>
    /// <response code="200">Confirma a aprova��o do TED.</response>
    /// <response code="400">Se o ID do TED � inv�lido ou ocorre um erro.</response>

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
    /// <param name="adminTedRequestDto">Motivo da reprova��o do TED.</param>
    /// <param name="id">ID do TED a ser reprovado.</param>
    /// <returns>Confirma��o da reprova��o do TED.</returns>
    /// <response code="200">Confirma a reprova��o do TED.</response>
    /// <response code="400">Se o ID do TED � inv�lido, os dados s�o inv�lidos ou ocorre um erro.</response>

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
    /// <param name="limiteTedRequestDto">Dados para atualiza��o do limite de TED.</param>
    /// <returns>Confirma��o da atualiza��o do limite de TED.</returns>
    /// <response code="200">Confirma a atualiza��o do limite de TED.</response>
    /// <response code="400">Se os dados s�o inv�lidos ou ocorre um erro.</response>
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
    /// Obt�m o limite atual para a realiza��o de TEDs.
    /// </summary>
    /// <returns>Uma resposta API contendo os detalhes do limite atual para TEDs, incluindo o valor do limite.</returns>
    /// <response code="200">Retorna o limite atual para TEDs, facilitando o monitoramento e ajustes necess�rios por parte dos administradores.</response>
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