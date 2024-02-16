using FluentValidation;
using TED.API.Parameters;

namespace TED.API.Validators
{
    public class AdminTedQueryParametersValidator : AbstractValidator<AdminTedQueryParameters>
    {
        public AdminTedQueryParametersValidator()
        {
            RuleFor(ted => ted.Status).IsInEnum().WithMessage("Status inválido.");

            RuleFor(x => x.ClienteId)
                .GreaterThan(0).WithMessage("ClientId deve ser maior que 0.");

            RuleFor(x => x)
                .Must(x => !x.DataInicio.HasValue || !x.DataFim.HasValue || x.DataInicio <= x.DataFim)
                .WithMessage("A data de início deve ser anterior ou igual à data de término.");
        }
    }
}
