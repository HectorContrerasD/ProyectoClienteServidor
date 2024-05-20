using DepartamentosAPI.Models.DTOS;
using FluentValidation;
using FluentValidation.Results;

namespace DepartamentosAPI.Models.Validators
{
    public static class LoginValidator
    {
        public static ValidationResult Validate(LoginDTO usuario)
        {
            var validator = new InlineValidator<LoginDTO>();
            validator.RuleFor(x => x.Nombre).NotEmpty().WithMessage("Escriba el correo electrónico del usuario");
            validator.RuleFor(x => x.Contrasena).NotEmpty().WithMessage("Escriba la contaseña");
            return validator.Validate(usuario);
        }
    }
}
