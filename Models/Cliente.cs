using System;
using System.Collections.Generic;

namespace GestionTurnos.Models;

public partial class Cliente
{
    public int Idcliente { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
