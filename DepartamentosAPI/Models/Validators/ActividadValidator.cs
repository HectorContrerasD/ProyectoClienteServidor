using DepartamentosAPI.Models.DTOS;
using FluentValidation;
using FluentValidation.Results;

namespace DepartamentosAPI.Models.Validators
{
    public static class ActividadValidator
    {
        public static ValidationResult Validate(ActividadCreateDTO Actividad)
        {
            var validator = new InlineValidator<ActividadCreateDTO>();
            validator.RuleFor(x=>x.Titulo)
                .NotEmpty().WithMessage("Se necesita el título de la actividad");
           
            validator.RuleFor(x => x.Estado).NotEmpty().WithMessage("Se necesita el estado");
            validator.RuleFor(x => x.Estado).GreaterThan(0).LessThan(3).WithMessage("El estado no existe");
        
            return validator.Validate(Actividad);
        }
    }
}
