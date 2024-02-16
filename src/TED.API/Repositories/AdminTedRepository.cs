using Microsoft.EntityFrameworkCore;
using TED.API.Context;
using TED.API.DTOs;
using TED.API.Entities;
using TED.API.Enums;
using TED.API.Interfaces;
using TED.API.Parameters;

namespace TED.API.Repositories
{
    public class AdminTedRepository: IAdminTedRepository
    {
        private readonly AppDBContext _context;

        public AdminTedRepository(
            AppDBContext context
        )
        {
            _context = context;
        }

        public async Task<(List<Ted>, int)> ObtemListaTedAsync(AdminTedQueryParameters queryParameters)
        {
            try
            {
                var query = _context.Ted.AsQueryable();

                if (queryParameters.ClienteId > 0)
                    query = query.Where(f => f.ClienteId == queryParameters.ClienteId);

                if (!string.IsNullOrEmpty(queryParameters.NomeCliente))
                    query = query.Where(f => f.NomeCliente.Contains(queryParameters.NomeCliente));

                if (!string.IsNullOrEmpty(queryParameters.NomeBanco))
                    query = query.Where(f => f.NomeBanco.Contains(queryParameters.NomeBanco));

                if(!queryParameters.DataInicio.HasValue)
                    queryParameters.DataInicio = DateTime.Now;

                if (!queryParameters.DataFim.HasValue)
                    queryParameters.DataFim = DateTime.Now;

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

        public async Task AprovaTedAsync(int id, string sinacorConfirmacaoId)
        {
            try
            {
                var ted = _context.Ted.Where(f => f.Id == id).AsNoTracking().FirstOrDefault();

                if (ted is null)
                    throw new Exception("nenhum Ted encontrado para o id informado.");

                if (ted.Status == StatusEnum.Approved)
                    throw new Exception("Ted já foi aprovado");

                if (ted.Status != StatusEnum.InProcess)
                    throw new Exception("Ted não pode mais ser aprovado");


                ted.Status = StatusEnum.Approved;
                ted.SinacorConfirmacaoId = sinacorConfirmacaoId;
                ted.AtualizadoEm = DateTime.Now;

                _context.Ted.Update(ted);
                await _context.SaveChangesAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task ReprovaTedAsync(int id, AdminTedRequestDto? adminTedRequestDto)
        {
            try
            {
                var ted = _context.Ted.Where(f => f.Id == id).AsNoTracking().FirstOrDefault();

                if (ted is null)
                    throw new Exception("nenhum Ted encontrado para o id informado.");

                if (ted.Status == StatusEnum.Disapproved)
                    throw new Exception("Ted já foi reprovado");

                if (ted.Status != StatusEnum.InProcess)
                    throw new Exception("Ted não pode mais ser reprovado");


                ted.Status = StatusEnum.Disapproved;
                ted.AtualizadoEm = DateTime.Now;
                ted.MotivoReprovacao = adminTedRequestDto?.MotivoReprovacao ?? null;

                _context.Ted.Update(ted);
                await _context.SaveChangesAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task AtualizaLimiteTedAsync(LimiteTed limiteTed)
        {
            try
            {
                var limiteTedResponse = await _context.LimiteTed.AsNoTracking().FirstOrDefaultAsync();

                limiteTedResponse.QuantidadeMaximaDia = limiteTed.QuantidadeMaximaDia;
                limiteTedResponse.ValorMaximoDia = limiteTed.ValorMaximoDia;
                limiteTedResponse.ValorMaximoPorSaque = limiteTed.ValorMaximoPorSaque;

                _context.LimiteTed.Update(limiteTedResponse);
                await _context.SaveChangesAsync();
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

        public async Task<LimiteTed?> ObtemLimiteTedAsync()
        {
            try
            {
                return await _context.LimiteTed.AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }
    }
}
