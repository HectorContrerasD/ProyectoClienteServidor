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
            validator.RuleFor(x => x.Departamento)
                .NotEmpty().WithMessage("Se neceita el departamento al que pertenece la actividad");
            validator.RuleFor(x => x.Estado).NotEmpty().WithMessage("Se necesita el estado");
            validator.RuleFor(x => x.Estado).GreaterThan(0).LessThan(3).WithMessage("El estado no existe");
        //    public int? Id { get; set; }
        //public string Titulo { get; set; } = null!;
        //public string? Descripcion { get; set; }
        //public string Departamento { get; set; } = null!;
        //public DateOnly? FechaRealizacion { get; set; } //cuando se realizó la actividad
        //public DateTime FechaCreacion { get; set; }
        //public DateTime FechaActualizacion { get; set; }
        //public int Estado { get; set; } 
            return validator.Validate(Actividad);
        }
    }
}
