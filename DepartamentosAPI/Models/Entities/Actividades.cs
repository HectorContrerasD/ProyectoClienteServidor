using System;
using System.Collections.Generic;

namespace DepartamentosAPI.Models.Entities;

public partial class Actividades
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateOnly? FechaRealizacion { get; set; }

    public int IdDepartamento { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int Estado { get; set; }

    public virtual Departamentos IdDepartamentoNavigation { get; set; } = null!;
}
