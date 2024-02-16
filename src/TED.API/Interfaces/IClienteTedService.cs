using TED.API.DTOs;
using TED.API.Parameters;
using TED.API.Response;

namespace TED.API.Interfaces
{
    public interface IClienteTedService
    {
        Task<PagedResponse<ClienteTedResponseDto>> ObtemListaTedAsync(int clienteId, ClienteTedQueryParameters queryParameters);
        Task<ClienteTedResponseDto> SolicitaTedAsync(ClienteTedRequestDto tedDto);
        Task CancelaTedAsync(int id);
    }
}
