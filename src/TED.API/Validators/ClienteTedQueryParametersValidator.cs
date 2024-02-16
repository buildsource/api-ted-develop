using FluentValidation;
using TED.API.Parameters;

namespace TED.API.Validators
{
    public class ClienteTedQueryParametersValidator : AbstractValidator<ClienteTedQueryParameters>
    {
        public ClienteTedQueryParametersValidator()
        {
            RuleFor(ted => ted.Status).IsInEnum().WithMessage("Status inválido.");

            RuleFor(x => x)
                .Must(x => !x.DataInicio.HasValue || !x.DataFim.HasValue || x.DataInicio <= x.DataFim)
                .WithMessage("A data de início deve ser anterior ou igual à data de término.");
        }
    }
}
