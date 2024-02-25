using System;
using System.Collections.Generic;

namespace GestionTurnos.Models;

public partial class DetalleTurno
{
    public int IdDetalleTurno { get; set; }

    public int? Idturno { get; set; }

    public int? Idservicio { get; set; }

    public decimal? Importe { get; set; }

    public bool? Anulado { get; set; }
}
