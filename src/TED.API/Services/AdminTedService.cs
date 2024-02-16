using AutoMapper;
using TED.API.DTOs;
using TED.API.Entities;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Parameters;
using TED.API.Response;
using TED.API.Validators;

namespace TED.API.Services
{
    public class AdminTedService : IAdminTedService
    {
        private readonly IAdminTedRepository _repository;
        private readonly IMapper _mapper;
        private readonly ISinacorTedService _sinacorTedService;

        public AdminTedService(
            IAdminTedRepository repository,
            IMapper mapper,
            ISinacorTedService sinacorTedService
        )
        {
            _repository = repository;
            _mapper = mapper;
            _sinacorTedService = sinacorTedService;
        }

        public async Task<PagedResponse<AdminTedResponseDto>> ObtemListaTedAsync(AdminTedQueryParameters queryParameters)
        {
            var validator = new AdminTedQueryParametersValidator();
            var validationResult = validator.Validate(queryParameters);


            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors.Select(e => e.ErrorMessage.TrimEnd('.')).ToList());

            try
            {
                var (teds, totalItems) = await _repository.ObtemListaTedAsync(queryParameters);
                var responseDtos = _mapper.Map<List<AdminTedResponseDto>>(teds);
                return new PagedResponse<AdminTedResponseDto>(responseDtos, totalItems, queryParameters.NumeroPagina, queryParameters.QuantidadeItensPagina);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task AprovaTedAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException(new List<string> { "O Id não é válido" });

            try
            {
                var ted = await _repository.ObtemTedPeloIdAsync(id);
                if(ted is null)
                    throw new Exception("não existe Ted com o Id informado.");


                var lancamentoId = await _repository.ObtemLancamentoIdTedAsync();

                var sinacorTedRequestDto = new SinacorTedRequestDto
                {
                    Lancamentos = new List<Lancamento>
                        {
                            {
                                new Lancamento
                                {
                                    IdLcto = lancamentoId,
                                    DataMovimento = ted.DataAgendamento.ToString("yyyy-MM-dd"),
                                    DataReferencia = ted.CriadoEm.ToString("yyyy-MM-dd"),
                                    CodigoCliente = ted.ClienteId,
                                    ValorLcto = (ted.ValorSolicitado > 0) ? -ted.ValorSolicitado : ted.ValorSolicitado,
                                    CodigoBancoCliente = ted.NumeroBanco,
                                    CodigoAgenciaCliente = ted.NumeroAgencia,
                                    NumeroContaCliente = ted.NumeroConta,
                                    DigitoContaCliente = ted.DigitoConta,
                                }
                            }
                        }
                };


                var sinacorConfirmacaoId = await _sinacorTedService.ClienteEnviaTedParaSinacorAsync(sinacorTedRequestDto);

                await _repository.AprovaTedAsync(id, sinacorConfirmacaoId);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task ReprovaTedAsync(int id, AdminTedRequestDto? adminTedRequestDto)
        {
            if (id <= 0)
                throw new ValidationException(new List<string> { "O Id não é válido" });

            try
            {
                var ted = await _repository.ObtemTedPeloIdAsync(id);
                if (ted is null)
                    throw new Exception("não existe Ted com o Id informado.");


                await _repository.ReprovaTedAsync(id, adminTedRequestDto);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task AtualizaLimiteTedAsync(LimiteTedRequestDto limiteTedRequestDto)
        {
            var validator = new LimiteTedRequestDtoValidator();
            var validationResult = validator.Validate(limiteTedRequestDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors.Select(e => e.ErrorMessage.TrimEnd('.')).ToList());

            
            try
            {
                var limiteTed = _mapper.Map<LimiteTed>(limiteTedRequestDto);

                await _repository.AtualizaLimiteTedAsync(limiteTed);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<LimiteTedResponseDto> ObtemLimiteTedAsync()
        {
            try
            {
                var response = await _repository.ObtemLimiteTedAsync();

                return _mapper.Map<LimiteTedResponseDto>(response);
            }
            catch (Exception _)
            {
                throw;
            }
        }
    }
}
