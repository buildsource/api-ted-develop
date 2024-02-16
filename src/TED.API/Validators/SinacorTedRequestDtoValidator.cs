using FluentValidation;
using TED.API.DTOs;

namespace TED.API.Validators;

public class SinacorTedRequestDtoValidator : AbstractValidator<SinacorTedRequestDto>
{
    public SinacorTedRequestDtoValidator()
    {
        RuleForEach(dto => dto.Lancamentos).SetValidator(new LancamentoValidator());
    }
}

public class LancamentoValidator : AbstractValidator<Lancamento>
{
    public LancamentoValidator()
    {
        RuleFor(dto => dto.DataReferencia)
            .NotEmpty().WithMessage("A data de referência é obrigatória.")
            .Must(BeAValidDate).WithMessage("A data de referência não é válida.")
            .Must(BeAPastOrPresentDate).WithMessage("A data de referência não pode ser no futuro.");

        RuleFor(dto => dto.CodigoCliente)
            .GreaterThan(0).WithMessage("O código do cliente deve ser maior que zero.");

        RuleFor(dto => dto.ValorLcto)
            .LessThan(0).WithMessage("O valor do lançamento deve ser menor que zero.");

        RuleFor(dto => dto.CodigoBanco)
            .NotEmpty().WithMessage("O código do banco é obrigatório.");

        RuleFor(dto => dto.CodigoAgencia)
            .NotEmpty().WithMessage("O código da agência é obrigatório.");

        RuleFor(dto => dto.NumeroConta)
            .GreaterThan(0).WithMessage("O número da conta deve ser maior que zero.");

        RuleFor(dto => dto.CodigoBancoCliente)
            .NotEmpty().WithMessage("O código do banco do cliente é obrigatório.");

        RuleFor(dto => dto.CodigoAgenciaCliente)
            .NotEmpty().WithMessage("O código da agência do cliente é obrigatório.");

        RuleFor(dto => dto.NumeroContaCliente)
            .NotEmpty().WithMessage("O número da conta do cliente é obrigatório.");

        RuleFor(dto => dto.DigitoContaCliente)
            .NotEmpty().WithMessage("O dígito da conta do cliente é obrigatório.");

        RuleFor(dto => dto.TipoContaCliente)
            .NotEmpty().WithMessage("O tipo da conta do cliente é obrigatório.");

        RuleFor(dto => dto.IndicadorSituacao)
            .NotEmpty().WithMessage("O indicador de situação é obrigatório.");

        RuleFor(dto => dto.CodigoSistemaExterno)
            .NotEmpty().WithMessage("O código do sistema externo é obrigatório.");
    }

    private bool BeAValidDate(string dataReferencia)
    {
        return DateTime.TryParse(dataReferencia, out _);
    }

    private bool BeAPastOrPresentDate(string dataReferencia)
    {
        if (DateTime.TryParse(dataReferencia, out DateTime parsedDate))
            return parsedDate.Date <= DateTime.Now.Date;
        return false;
    }
}