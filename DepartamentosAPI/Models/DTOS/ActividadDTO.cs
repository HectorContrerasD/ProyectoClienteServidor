namespace DepartamentosAPI.Models.DTOS
{
    public class ActividadDTO
    {
        public int? Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; } 
        public string Departamento { get; set; } = null!;
        public DateOnly? FechaRealizacion {  get; set; } //cuando se realizó la actividad
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion {get; set; } 
       
    }
}
