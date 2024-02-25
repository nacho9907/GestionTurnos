using System;
using System.Collections.Generic;

namespace GestionTurnos.Models;

public partial class Servicio
{
    public int Idservicio { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public int? DuracionEstimada { get; set; }

    public decimal? Costo { get; set; }

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
