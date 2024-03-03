using Microsoft.AspNetCore.Mvc;
using GestionTurnos.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace GestionTurnos.Controllers
{
    public class TurnosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public TurnosController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        #region VISTA LISTAR TURNOS
        // GET: /Turnos/Index
        public ActionResult Index()
        {
            var turnos = new List<TurnoViewModel>();
            var listaServicios = _context.Servicios.Where(s => s.Anulado == null).ToList();
            ViewBag.ListaServicios = listaServicios;

            string storedProcedureName = "ObtenerTurnosConCliente";
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

            // Obtener la lista de servicios desde la base de datos


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TurnoViewModel turno = new TurnoViewModel
                                {
                                    Idturno = (int)reader["IDTurno"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    Hora = (TimeSpan)reader["Hora"],
                                    Estado = reader["Estado"].ToString(),
                                    NombreCliente = reader["NombreCliente"].ToString(),
                                    ApellidoCliente = reader["ApellidoCliente"].ToString(),

                                };

                                turnos.Add(turno);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, registrándola o devolviendo una vista de error
                // Log.Error("Error al obtener los turnos", ex);
                return View("Error");
            }

            return View(turnos);
        }
        public class TurnoViewModel
        {
            public int Idturno { get; set; }
            public DateTime Fecha { get; set; }
            public TimeSpan Hora { get; set; }
            public string Estado { get; set; }
            public string NombreCliente { get; set; }
            public string ApellidoCliente { get; set; }
            public int Idcliente { get; set; }
            public int Idservicio { get; set; }
            public int IdDetalleTurno { get; set; }
            public decimal Importe { get; set; }
        }

        #endregion

        #region CREAR TURNO
        // POST: /Turnos/Crear
        [HttpPost]
        public ActionResult Crear([FromBody] TurnoCrear turno)
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

                    //variables estaticas de prueba
                    decimal Importe_ = 100;

                    // Crear un comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("InsertarTurnoYDetalleTurno", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Pasar los parámetros al procedimiento almacenado
                        command.Parameters.AddWithValue("@IDCliente", turno.Idcliente);
                        command.Parameters.AddWithValue("@IDServicio", turno.Idservicio);
                        command.Parameters.AddWithValue("@Fecha", turno.Fecha);
                        command.Parameters.AddWithValue("@Hora", turno.Hora);
                        command.Parameters.AddWithValue("@Estado", turno.Estado);
                        command.Parameters.AddWithValue("@Importe", Importe_);


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
                return BadRequest($"Error al crear el turno: {ex.Message}");
            }

        }

        [JsonObject(MemberSerialization.OptIn)]
        public class TurnoCrear
        {
            public int Idcliente { get; set; }
            public int Idservicio { get; set; }
            public DateTime Fecha { get; set; }
            public TimeSpan Hora { get; set; }
            public string? Estado { get; set; }
        }

        #endregion

        #region ANULAR TURNO
        public class TurnoAnularModel
        {
            public int Idturno { get; set; }

        }

        [HttpPost]
        public ActionResult ActualizarTurnoComoAnulado([FromBody] TurnoAnularModel model)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {
                int Idturno = model.Idturno;

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("ActualizarTurnoComoAnulado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Idturno", Idturno);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region BUSCAR DATOS DE CLIENTE POR CODIGO DE CLIENTE


        [HttpPost]
        public IActionResult BuscarCliente([FromBody] int CodigoCliente)
        {
            try
            {
                var clienteExiste = _context.Clientes
                    .Where(c => c.Idcliente == CodigoCliente && c.Anulado == null)
                    .ToList();

                if (clienteExiste.Any())
                {
                    // El cliente existe, devuelve la lista como resultado de la solicitud de Axios
                    return Ok(clienteExiste);
                }
                else
                {
                    // El cliente no existe, devuelve un resultado indicando que no se encontró
                    return Ok(new { mensaje = "Cliente no encontrado" });
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, registrándola o devolviendo un error interno del servidor
                // Log.Error("Error al obtener los datos del cliente", ex);
                return StatusCode(500, "Error interno del servidor");
            }
        }



        #endregion

        #region MARCAR ESTADO COMPLETADO 
        public ActionResult MarcarTurnoCompletado([FromBody] int IDTurno)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("ActualizarEstadoTurnoCompletado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDTurno", IDTurno);

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
        #endregion

        #region MARCAR ESTADO EN ESPERA
        public ActionResult MarcarTurnoEspera([FromBody] int IDTurno)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("ActualizarEstadoTurnoEspera", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDTurno", IDTurno);

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
        #endregion

        #region LISTAR DETALLES DEL TURNO
        public ActionResult DetallesTurno([FromBody] int IDTurno)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {
                // Lista para almacenar los resultados del stored procedure
                List<DetallesTurnoViewModel> detallesTurnoList = new List<DetallesTurnoViewModel>();

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("ObtenerDetalleTurnoConServicio", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDTurno", IDTurno);

                        connection.Open();

                        // Utiliza un lector de datos para obtener los resultados del stored procedure
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Crea un objeto DetallesTurnoViewModel para cada fila devuelta por el stored procedure
                                DetallesTurnoViewModel detalle = new DetallesTurnoViewModel
                                {
                                    // Ajusta las propiedades según las columnas devueltas por el stored procedure
                                    IdDetalleTurno = Convert.ToInt32(reader["IdDetalleTurno"]),
                                    IDTurno = Convert.ToInt32(reader["IDTurno"]),
                                    IDServicio = Convert.ToInt32(reader["IDServicio"]),
                                    Nombre = Convert.ToString(reader["Nombre"]),
                                    Costo = Convert.ToDecimal(reader["Costo"]),
                                    DuracionEstimada = Convert.ToInt32(reader["DuracionEstimada"])
                                };

                                detallesTurnoList.Add(detalle);
                            }
                        }
                    }
                }
                return Json(detallesTurnoList);

            }
            catch (Exception ex)
            {
                //en caso de error redirigir a index
                return Json(new { error = ex.Message });
            }
        }

        public class DetallesTurnoViewModel
        {
            public int IdDetalleTurno { get; set; }
            public int IDTurno { get; set; }
            public int IDServicio { get; set; }
            public bool? Anulado { get; set; }
            public string? Nombre { get; set; }
            public decimal Costo { get; set; }
            public int DuracionEstimada { get; set; }
        }

        #endregion

        #region CARGAR SERVICIO AL TURNO
        public IActionResult CargarServicio([FromBody] CargarServicioData data)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
            try
            {

                // Llamada al stored procedure
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("AgregarServicioADetalleTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDTurno", data.IDTurno);
                        command.Parameters.AddWithValue("@IDServicio", data.IDServicio);


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

        public class CargarServicioData
        {
            public int IDTurno { get; set; }
            public int IDServicio { get; set; }
        }

        #endregion
        public class TurnoEstadoModel
        {
            public int IDTurno { get; set; }
        }
    }
}