using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Controllers
{
    public class ServiciosController : Controller
    {
        public IActionResult Index()  {
        
            return View();

        }
    }
}
