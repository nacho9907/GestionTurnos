using GestionTurnos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GestionTurnos.Controllers
{
    public class TurnosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TurnosController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Poronga()
        {
            int IDServicio = 1;
            var turno = _context.Servicios.FirstOrDefault(t => t.Idservicio == IDServicio);
            if (turno != null)
            {
                return Ok(turno);
            }
            return NotFound(); // Devuelve un código 404 si no se encuentra ningún turno con el ID de servicio especificado.
        }

    }
}

