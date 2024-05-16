namespace DepartamentosAPI.Models.DTOS
{
    public class DepartamentoCreateDTO
    {
        public int? Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Usuario { get; set; } = null!;

        public string Contraseña { get; set; } = null!;

        public int? IdSuperior { get; set; }


    }
}
