namespace DepartamentosAPI.Models.DTOS
{
    public class ActividadCreateDTO
    {
        public int? Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
       
        public DateOnly? FechaRealizacion { get; set; } //cuando se realizó la actividad
        public string? Imagen { get; set; }
    }
}
