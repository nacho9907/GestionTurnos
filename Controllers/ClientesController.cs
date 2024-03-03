using Microsoft.AspNetCore.Mvc;
using GestionTurnos.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace GestionTurnos.Controllers
{   
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ClientesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: ClientesController
        public ActionResult Index()
        {
            var Clientes = _context.Clientes.Where(s => s.Anulado == null).ToList();
            return View(Clientes);
        }

        // POST: ClientesController/Create
        [HttpPost]
        public IActionResult Crear([FromBody] Cliente cliente)
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
                        using (SqlCommand command = new SqlCommand("InsertarCliente", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            // Pasar los parámetros al procedimiento almacenado
                            command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                            command.Parameters.AddWithValue("@Apellido ", cliente.Apellido);
                            command.Parameters.AddWithValue("@Telefono ", cliente.Telefono);
                            command.Parameters.AddWithValue("@Correo ", cliente.CorreoElectronico);

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
            return View(cliente);
        }
        public class ClienteAnularModel
        {
            public int IDCliente { get; set; }
        }

        [HttpPost]
        public ActionResult ActualizarClienteComoAnulado([FromBody] ClienteAnularModel model)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {
                int IDCliente = model.IDCliente;

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("ActualizarClienteComoAnulado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdCliente", IDCliente);

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


    }
}
