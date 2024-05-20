using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Entities;
using FluentValidation;
using FluentValidation.Results;


namespace DepartamentosAPI.Models.Validators

{
    public static class DepartamentoValidator
    {
        public static ValidationResult Validate(DepartamentoCreateDTO departamento, ItesrcneActividadesContext context)
        {
            var validator = new InlineValidator<DepartamentoCreateDTO>();
            validator.RuleFor(x => x.Nombre)
              .NotEmpty().WithMessage("El nombre del departamento no puede estar vacío")
              .Must((dto, nombre) => ExisteDepartamento(nombre, context))
              .WithMessage("Ya existe un departamento con este nombre");
            validator.RuleFor(x => x.Usuario)
                .NotEmpty().WithMessage("El nombre de usuario no puede estar vacío")
                .Must((dto, nombre) => ExisteUsuario(departamento, context)).WithMessage("Este usuario ya existe")
                .Must(usuario => TieneExtensionCorrecta(usuario))
                .WithMessage("El nombre de usuario debe tener la extensión '@equipo9.com'");
                
            validator.RuleFor(x=>x.Contraseña).NotEmpty().WithMessage("La contraseña no puede estar vacia");

            return validator.Validate(departamento);
        }

        private static bool ExisteDepartamento(string nombre, ItesrcneActividadesContext context)
        {
            return !context.Departamentos.Any(d => d.Nombre == nombre);
        }

        private static bool ExisteUsuario(DepartamentoCreateDTO departamento, ItesrcneActividadesContext context)
        {
            return !context.Departamentos.Any(d => d.Username == departamento.Usuario && d.Id == departamento.Id);
        }
        private static bool TieneExtensionCorrecta(string usuario)
        {
            return usuario.EndsWith("@equipo9.com", StringComparison.OrdinalIgnoreCase);
        }
    }
}
