using TED.API.Entities;
using TED.API.Parameters;

namespace TED.API.Interfaces
{
    public interface IClienteTedRepository
    {
        Task<(List<Ted>, int)> ObtemListaTedAsync(int clienteId, ClienteTedQueryParameters queryParameters);
        Task<Ted> SolicitaTedAsync(Ted tedDto);
        Task CancelaTedAsync(int id);
        Task<List<Ted>> VerificaLimiteTedDoDiaAsync(int clientId);
        Task<Ted?> ObtemTedPeloIdAsync(int id);
        Task<LimiteTed> ObtemLimiteTedAsync();
        Task<int> ObtemLancamentoIdTedAsync();
    }
}
