using FluentValidation;
using TED.API.DTOs;

namespace TED.API.Validators
{
    public class ClienteTedRequestDtoValidator : AbstractValidator<ClienteTedRequestDto>
    {
        public ClienteTedRequestDtoValidator()
        {
            RuleFor(ted => ted.ClienteId).GreaterThan(0).WithMessage("ClientId deve ser maior que 0.");

            RuleFor(ted => ted.NomeCliente)
                .NotEmpty()
                .WithMessage("Nome do cliente não pode estar vazio.");

            RuleFor(ted => ted.DataAgendamento)
                .GreaterThanOrEqualTo(DateTime.Now.Date)
                .WithMessage("A data de agendamento não pode ser no passado.");

            RuleFor(ted => ted.ValorSolicitado).GreaterThan(0).WithMessage("O valor deve ser maior que 0.");

            RuleFor(ted => ted.NumeroAgencia)
            .Must(BeANonZeroNumber).WithMessage("Número da agência deve ser um número maior que 0.");

            RuleFor(ted => ted.NumeroConta)
                .Must(BeANonZeroNumber).WithMessage("Número da conta deve ser um número maior que 0.");

            RuleFor(ted => ted.DigitoConta)
                .Must(BeANonNegativeInteger).WithMessage("Dígito da conta deve ser um número maior ou igual 0.");

            RuleFor(ted => ted.NumeroBanco)
                .Must(BeANonZeroNumber).WithMessage("Número do banco deve ser um número maior que 0.");

            RuleFor(ted => ted.NomeBanco)
                .NotEmpty()
                .WithMessage("Nome do banco não pode estar vazio.");

        }

        private bool BeANonZeroNumber(string numberString)
        {
            if (int.TryParse(numberString, out int number))
                return number > 0;
            return false;
        }

        private bool BeANonNegativeInteger(string numberString)
        {
            if (int.TryParse(numberString, out int number))
                return number >= 0;
            return false;
        }

    }
}
