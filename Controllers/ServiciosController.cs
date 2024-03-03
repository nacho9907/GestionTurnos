using Microsoft.AspNetCore.Mvc;
using GestionTurnos.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GestionTurnos.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ServiciosController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var servicios = _context.Servicios.Where(s => s.Anulado == null).ToList();
            return View(servicios);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la cadena de conexión desde la configuración
                    string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

                    // Crear una nueva conexión SQL utilizando la cadena de conexión
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Abrir la conexión
                        connection.Open();

                        // Crear un comando para ejecutar el procedimiento almacenado
                        using (SqlCommand command = new SqlCommand("InsertarServicio", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            // Pasar los parámetros al procedimiento almacenado
                            command.Parameters.AddWithValue("@Nombre", servicio.Nombre);
                            command.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
                            command.Parameters.AddWithValue("@DuracionEstimada", servicio.DuracionEstimada);
                            command.Parameters.AddWithValue("@Costo", servicio.Costo);

                            // Ejecutar el comando
                            command.ExecuteNonQuery();
                        }
                    }

                    // Redirigir a la acción Index para mostrar la lista actualizada de servicios
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    // Manejar cualquier error que pueda ocurrir durante la ejecución del procedimiento almacenado
                    // Aquí podrías redirigir a una página de error o mostrar un mensaje de error al usuario
                    return BadRequest($"Error al crear el servicio: {ex.Message}");
                }
            }
            return View(servicio);
        }

        public class ServicioAnularModel
        {
            public int IDServicio { get; set; }
        }

        [HttpPost]
        public ActionResult ActualizarServicioComoAnulado([FromBody] ServicioAnularModel model)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {
                int IDServicio = model.IDServicio;

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("ActualizarServicioComoAnulado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDServicio", IDServicio);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                // Redirigir a donde desees después de la actualización
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Manejar cualquier error que pueda ocurrir durante la ejecución del stored procedure
                // Puedes redirigir a una página de error o mostrar un mensaje de error al usuario
                return RedirectToAction("Index"); // O redirige a una página de error
            }
        }

        //[HttpPost]
        //public IActionResult ActualizarServicio(int IDServicio, string Nombre, string Descripcion, int DuracionEstimada, decimal Costo)
        //{
        //    try
        //    {
        //        // Obtener la cadena de conexión desde la configuración
        //        string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

        //        // Crear una nueva conexión SQL utilizando la cadena de conexión
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            // Abrir la conexión
        //            connection.Open();

        //            // Crear un comando para ejecutar el procedimiento almacenado
        //            using (SqlCommand command = new SqlCommand("ActualizarServicioCompleto", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;

        //                // Pasar los parámetros al procedimiento almacenado
        //                command.Parameters.AddWithValue("@IDServicio", IDServicio);
        //                command.Parameters.AddWithValue("@Nombre", Nombre);
        //                command.Parameters.AddWithValue("@Descripcion", Descripcion);
        //                command.Parameters.AddWithValue("@DuracionEstimada", DuracionEstimada);
        //                command.Parameters.AddWithValue("@Costo", Costo);

        //                // Ejecutar el comando
        //                command.ExecuteNonQuery();
        //            }
        //        }

        //        // Redirigir a donde desees después de la actualización
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Manejar cualquier error que pueda ocurrir durante la ejecución del procedimiento almacenado
        //        // Puedes redirigir a una página de error o mostrar un mensaje de error al usuario
        //        return BadRequest($"Error al actualizar el servicio: {ex.Message}");
        //    }
        //}
    }
}
