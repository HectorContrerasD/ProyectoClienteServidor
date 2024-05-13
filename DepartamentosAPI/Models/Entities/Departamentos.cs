using System;
using System.Collections.Generic;

namespace DepartamentosAPI.Models.Entities;

public partial class Departamentos
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? IdSuperior { get; set; }

    public virtual ICollection<Actividades> Actividades { get; set; } = new List<Actividades>();

    public virtual Departamentos? IdSuperiorNavigation { get; set; }

    public virtual ICollection<Departamentos> InverseIdSuperiorNavigation { get; set; } = new List<Departamentos>();
}
