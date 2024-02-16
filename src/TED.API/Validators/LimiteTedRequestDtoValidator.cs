using FluentValidation;
using TED.API.DTOs;

namespace TED.API.Validators
{
    public class LimiteTedRequestDtoValidator : AbstractValidator<LimiteTedRequestDto>
    {
        public LimiteTedRequestDtoValidator()
        {
            RuleFor(x => x.ValorMaximoDia)
                .GreaterThan(0).WithMessage("O valor máximo por dia deve ser maior que 0.");

            RuleFor(x => x.QuantidadeMaximaDia)
                .GreaterThan(0).WithMessage("A quantidade máxima por dia deve ser maior que 0.");

            RuleFor(x => x.ValorMaximoPorSaque)
                .GreaterThan(0).WithMessage("O valor máximo por saque deve ser maior que 0.");
        }
    }
}
