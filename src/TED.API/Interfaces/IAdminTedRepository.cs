using TED.API.DTOs;
using TED.API.Entities;
using TED.API.Parameters;

namespace TED.API.Interfaces
{
    public interface IAdminTedRepository
    {
        Task<(List<Ted>, int)> ObtemListaTedAsync(AdminTedQueryParameters queryParameters);
        Task AprovaTedAsync(int id, string sinacorConfirmacaoId);
        Task ReprovaTedAsync(int id, AdminTedRequestDto? adminTedRequestDto);
        Task<Ted?> ObtemTedPeloIdAsync(int id);
        Task AtualizaLimiteTedAsync(LimiteTed limite);
        Task<int> ObtemLancamentoIdTedAsync();
        Task<LimiteTed?> ObtemLimiteTedAsync();
    }
}
