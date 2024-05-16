namespace DepartamentosAPI.Models.DTOS
{
    public class DepartamentoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set;}= null!;
        public string? DepartamentoSuperior { get; set; } 
    }
}
