using System;
using System.Collections.Generic;

namespace GestionTurnos.Models;

public partial class Turno
{
    public int Idturno { get; set; }

    public int? Idcliente { get; set; }

    public int? Idservicio { get; set; }

    public DateOnly? Fecha { get; set; }

    public TimeOnly? Hora { get; set; }

    public string? Estado { get; set; }

    public int IdDetalleTurno { get; set; }

    public bool? Anulado { get; set; }

    public virtual Cliente? IdclienteNavigation { get; set; }

    public virtual Servicio? IdservicioNavigation { get; set; }
}