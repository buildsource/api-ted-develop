using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;
using System;
using TED.API.DTOs;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Parameters;
using TED.API.Response;

namespace TED.API.Controllers;

/// <summary>
/// Controlador respons�vel por opera��es relacionadas a TEDs de clientes.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly ILogger<ClienteController> _logger;
    private readonly IClienteTedService _service;

    public ClienteController(
        ILogger<ClienteController> logger,
        IClienteTedService service
    )
    {
        _logger = logger;
        _service = service;
    }


    /// <summary>
    /// Obt�m uma lista paginada de TEDs associados a um cliente espec�fico. 
    /// Esta opera��o permite filtrar e recuperar TEDs com base em crit�rios definidos nos par�metros de consulta, 
    /// juntamente com a informa��o do cliente. A resposta � paginada para facilitar a gest�o de grandes volumes de dados.
    /// </summary>
    /// <param name="queryParameters">Par�metros de consulta para filtrar a lista paginada de TEDs. 
    /// Inclui op��es como n�mero da p�gina, tamanho da p�gina e outros filtros espec�ficos.</param>
    /// <param name="clienteId">Identificador �nico do cliente para o qual a lista de TEDs ser� obtida. 
    /// Este par�metro especifica para qual cliente as TEDs devem ser filtradas.</param>
    /// <returns>Uma resposta API contendo a lista paginada de TEDs associadas ao cliente especificado, 
    /// com detalhes adicionais de pagina��o como n�mero total de itens e p�ginas.</returns>
    /// <response code="200">Retorna a lista paginada de TEDs do cliente especificado, 
    /// incluindo detalhes sobre o total de itens e o n�mero total de p�ginas, baseando-se nos crit�rios de consulta fornecidos.</response>
    /// <response code="400">Retorna um erro se os par�metros de consulta s�o inv�lidos, 
    /// se o ID do cliente � inv�lido, ou se ocorre um erro durante a obten��o dos dados.</response>

    [HttpGet("lista-ted/{clienteId}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ClienteTedResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtemTedAsync([FromQuery] ClienteTedQueryParameters queryParameters, int clienteId)
    {
        try
        {
            var teds = await _service.ObtemListaTedAsync(clienteId, queryParameters);

            return Ok(new ApiResponse<PagedResponse<ClienteTedResponseDto>>(teds, "Ted listado com sucesso"));
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
    /// Solicita um novo TED.
    /// </summary>
    /// <param name="tedDto">Dados do TED a ser solicitado.</param>
    /// <returns>Detalhes do TED solicitado.</returns>
    /// <response code="200">Retorna os detalhes do TED solicitado.</response>
    /// <response code="400">Se os dados do TED s�o inv�lidos ou ocorre um erro.</response>
    
    [HttpPost("solicita-ted")]
    [ProducesResponseType(typeof(ApiResponse<ClienteTedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SolicitaTedAsync([FromBody] ClienteTedRequestDto tedDto)
     {
        try
        {
            var ted = await _service.SolicitaTedAsync(tedDto);

            return Ok(new ApiResponse<ClienteTedResponseDto>(ted, "Ted solicitado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao solicitar o Ted", new List<string> { ex.Message }));
        }
    }


    /// <summary>
    /// Cancela um TED pelo Id.
    /// </summary>
    /// <param name="id">ID do TED a ser cancelado.</param>
    /// <returns>Confirma��o do cancelamento do TED.</returns>
    /// <response code="200">Confirma o cancelamento do TED.</response>
    /// <response code="400">Se o ID do TED � inv�lido ou ocorre um erro.</response>
    
    [HttpPut("cancela-ted/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteTedResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<Notification>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelaTedAsync(int id)
    {
        try
        {
            await _service.CancelaTedAsync(id);

            return Ok(new ApiResponse<ClienteTedResponseDto>(null, "Ted cancelado com sucesso"));
        }
        catch (ValidationException vex)
        {
            return BadRequest(new ApiResponse<List<Notification>>(vex));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<Notification>>("Ocorreu um erro ao cancelar o Ted", new List<string> { ex.Message }));
        }
    }
}