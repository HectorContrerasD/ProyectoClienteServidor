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
           
          
        
            return validator.Validate(Actividad);
        }
    }
}
