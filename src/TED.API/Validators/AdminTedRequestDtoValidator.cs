using FluentValidation;
using TED.API.DTOs;

namespace TED.API.Validators
{
    public class AdminTedRequestDtoValidator : AbstractValidator<AdminTedRequestDto>
    {
        public AdminTedRequestDtoValidator()
        {
            RuleFor(ted => ted.MotivoReprovacao)
                .MaximumLength(500).WithMessage("Motivo de reprovação deve ter no máximo 500 caracteres.");
        }
    }
}
