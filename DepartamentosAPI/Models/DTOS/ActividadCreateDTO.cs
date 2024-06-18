namespace DepartamentosAPI.Models.DTOS
{
    public class ActividadCreateDTO
    {
        public int? Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int Departamento { get; set; }
        public DateOnly? FechaRealizacion { get; set; } //cuando se realizó la actividad
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int Estado { get; set; } = 1;
        public string Imagen { get; set; }
    }
}
