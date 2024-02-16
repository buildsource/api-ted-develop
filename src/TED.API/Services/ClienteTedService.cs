using AutoMapper;
using Microsoft.Extensions.Options;
using TED.API.DTOs;
using TED.API.Entities;
using TED.API.Exceptions;
using TED.API.Interfaces;
using TED.API.Parameters;
using TED.API.Response;
using TED.API.Validators;

namespace TED.API.Services
{
    public class ClienteTedService : IClienteTedService
    {
        private readonly IClienteTedRepository _repository;
        private readonly IMapper _mapper;
        private readonly ISinacorTedService _sinacorTedService;
        private readonly SinacorConfiguration _sinacorConfiguration;

        public ClienteTedService(
            IClienteTedRepository repository,
            IMapper mapper,
            ISinacorTedService sinacorTedService,
            IOptions<SinacorConfiguration> sinacorConfiguration
        )
        {
            _repository = repository;
            _mapper = mapper;
            _sinacorTedService = sinacorTedService;
            _sinacorConfiguration = sinacorConfiguration.Value;
        }

        public async Task<PagedResponse<ClienteTedResponseDto>> ObtemListaTedAsync(int clienteId, ClienteTedQueryParameters queryParameters)
        {
            if (clienteId <= 0)
                throw new ValidationException(new List<string> { "O ClienteId não é válido" });

            var validator = new ClienteTedQueryParametersValidator();
            var validationResult = validator.Validate(queryParameters);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors.Select(e => e.ErrorMessage.TrimEnd('.')).ToList());

            try
            {
                var (teds, totalItems) = await _repository.ObtemListaTedAsync(clienteId, queryParameters);
                var responseDtos = _mapper.Map<List<ClienteTedResponseDto>>(teds);
                return new PagedResponse<ClienteTedResponseDto>(responseDtos, totalItems, queryParameters.NumeroPagina, queryParameters.QuantidadeItensPagina);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<ClienteTedResponseDto> SolicitaTedAsync(ClienteTedRequestDto tedDto)
        {
            var validator = new ClienteTedRequestDtoValidator();
            var validationResult = validator.Validate(tedDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors.Select(e => e.ErrorMessage.TrimEnd('.')).ToList());


            try
            {
                var ted = _mapper.Map<Ted>(tedDto);

                var limiteTed = await VerificaLimiteTedDoDiaAsync(tedDto);
                if (!limiteTed && ted.DataAgendamento >= _sinacorConfiguration.HorarioInicio && ted.DataAgendamento <= _sinacorConfiguration.HorarioFim)
                {
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

                    ted.SinacorConfirmacaoId = await _sinacorTedService.ClienteEnviaTedParaSinacorAsync(sinacorTedRequestDto);

                    ted.Status = Enums.StatusEnum.Approved;
                }



                var tedResponse = await _repository.SolicitaTedAsync(ted);

                return _mapper.Map<ClienteTedResponseDto>(tedResponse);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task CancelaTedAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException(new List<string> { "O Id não é válido" });

            try
            {
                var ted = await _repository.ObtemTedPeloIdAsync(id);
                if (ted is null)
                    throw new Exception("não existe Ted com o Id informado.");


                await _repository.CancelaTedAsync(id);
            }
            catch (Exception _)
            {
                throw;
            }
        }

        private async Task<bool> VerificaLimiteTedDoDiaAsync(ClienteTedRequestDto tedDto)
        {
            var limite = await _repository.ObtemLimiteTedAsync();

            var tedsHoje = await _repository.VerificaLimiteTedDoDiaAsync(tedDto.ClienteId);

            if (tedsHoje.Count >= limite.QuantidadeMaximaDia)
                return true;

            if ((tedsHoje.Sum(t => t.ValorSolicitado)) + tedDto.ValorSolicitado > limite.ValorMaximoDia)
                return true;

            if (tedDto.ValorSolicitado > limite.ValorMaximoPorSaque)
                return true;

            return false;
        }
    }
}
