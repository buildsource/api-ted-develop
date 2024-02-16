using TED.API.DTOs;
using TED.API.Entities;
using TED.API.Parameters;
using TED.API.Response;

namespace TED.API.Interfaces
{
    public interface IAdminTedService
    {
        Task<PagedResponse<AdminTedResponseDto>> ObtemListaTedAsync(AdminTedQueryParameters queryParameters);
        Task AprovaTedAsync(int id);
        Task ReprovaTedAsync(int id, AdminTedRequestDto? adminTedRequestDto);
        Task AtualizaLimiteTedAsync(LimiteTedRequestDto limiteTedRequestDto);
        Task<LimiteTedResponseDto> ObtemLimiteTedAsync();
    }
}
