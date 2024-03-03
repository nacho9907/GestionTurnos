using System;
using System.Collections.Generic;

namespace GestionTurnos.Models;

public class Servicio
{
    public int IDServicio { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public int? DuracionEstimada { get; set; }

    public decimal? Costo { get; set; }

    public bool? Anulado { get; set; }

    //public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
