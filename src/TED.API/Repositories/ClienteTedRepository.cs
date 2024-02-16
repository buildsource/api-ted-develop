using Microsoft.EntityFrameworkCore;
using TED.API.Context;
using TED.API.Entities;
using TED.API.Enums;
using TED.API.Interfaces;
using TED.API.Parameters;

namespace TED.API.Repositories
{
    public class ClienteTedRepository: IClienteTedRepository
    {
        private readonly AppDBContext _context;

        public ClienteTedRepository(
            AppDBContext context
        )
        {
            _context = context;
        }

        public async Task<(List<Ted>, int)> ObtemListaTedAsync(int clienteId, ClienteTedQueryParameters queryParameters)
        {
            try
            {
                var query = _context.Ted.AsQueryable();

                query = _context.Ted.Where(f => f.ClienteId == clienteId);

                if (queryParameters.DataInicio.HasValue)
                    query = query.Where(f => f.CriadoEm.Date >= queryParameters.DataInicio.Value.Date);

                if (queryParameters.DataFim.HasValue)
                    query = query.Where(f => f.CriadoEm.Date <= queryParameters.DataFim.Value.Date);

                if (queryParameters.Status.HasValue)
                    query = query.Where(f => f.Status == queryParameters.Status);

                int totalItems = await query.CountAsync();

                var items = await query.Skip((queryParameters.NumeroPagina - 1) * queryParameters.QuantidadeItensPagina)
                                       .Take(queryParameters.QuantidadeItensPagina)
                                       .AsNoTracking()
                                       .ToListAsync();

                return (items, totalItems);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<Ted> SolicitaTedAsync(Ted ted)
        {
            try
            {
                _context.Ted.Add(ted);
                await _context.SaveChangesAsync();

                return ted;
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<Ted?> ObtemTedPeloIdAsync(int id)
        {
            try
            {
                return await _context.Ted.Where(f => f.Id == id).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task CancelaTedAsync(int id)
        {
            try
            {
                var ted = _context.Ted.Where(f => f.Id == id).AsNoTracking().FirstOrDefault();

                if (ted is null)
                    throw new Exception("nenhum Ted encontrado para o id informado.");

                if (ted.Status == StatusEnum.Canceled)
                    throw new Exception("Ted já foi cancelardo");

                if (ted.Status != StatusEnum.InProcess)
                    throw new Exception("Ted não pode mais ser cancelardo");


                ted.Status = StatusEnum.Canceled;
                ted.AtualizadoEm = DateTime.Now;

                _context.Ted.Update(ted);
                await _context.SaveChangesAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<List<Ted>> VerificaLimiteTedDoDiaAsync(int clientId)
        {
            try
            {
                return await _context.Ted
                    .Where(t => t.ClienteId == clientId && t.DataAgendamento.Date == DateTime.Now.Date)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<LimiteTed> ObtemLimiteTedAsync()
        {
            try
            {
                var limite = await _context.LimiteTed.FirstOrDefaultAsync();
                if (limite == null)
                    throw new Exception("Os limites não podem ser nulos");

                return limite;
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<int> ObtemLancamentoIdTedAsync()
        {
            try
            {
                var ultimoIdInserido = await _context.Ted
                    .AsNoTracking()
                    .OrderByDescending(t => t.Id)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefaultAsync();

                return ultimoIdInserido.HasValue ? ultimoIdInserido.Value + 1 : 1;
            }
            catch (Exception _)
            {
                throw;
            }
        }
    }
}
