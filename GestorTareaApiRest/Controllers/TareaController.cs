using GestorTareaApiRest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using GestorTareaApiRest.DTOs;

namespace GestorTareaApiRest.Controllers
{
    [ApiController]
    [Route("/")]
    [Authorize]
    public class TareaController : Controller
    {
        private readonly IConfiguration _configuration;

        public TareaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.FindFirst("usuarioId")?.Value;
            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedAccessException("El token no contiene el claim 'usuarioId'.");
            return int.Parse(claim);
        }

        /// <summary>
        /// Obtiene todas las tareas registradas en el sistema. No requiere autenticación.
        /// Permite filtrar por estado (completadas o no) y usar paginación.
        /// </summary>
        /// <param name="completado">True para tareas completadas, false para pendientes, null para todas.</param>
        /// <param name="page">Número de página a consultar.</param>
        /// <param name="limit">Cantidad de tareas por página.</param>
        /// <returns>Lista de tareas.</returns>
        [HttpGet("Tareas")]
        [AllowAnonymous]
        public IActionResult ObtenerTareas([FromQuery] bool? completado, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var tareas = new List<Tarea>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_ListarTareas", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Completado", (object?)completado ?? DBNull.Value);
            command.Parameters.AddWithValue("@Pagina", page);
            command.Parameters.AddWithValue("@Limite", limit);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tareas.Add(new Tarea
                {
                    Id = (int)reader["Id"],
                    Titulo = reader["Titulo"].ToString(),
                    Descripcion = reader["Descripcion"].ToString(),
                    Completado = (bool)reader["Completado"],
                    UsuarioId = (int)reader["UsuarioId"]
                });
            }

            return Ok(tareas);
        }

        /// <summary>
        /// Obtiene una tarea específica por su ID, validando que pertenezca al usuario autenticado.
        /// </summary>
        /// <param name="id">ID de la tarea a buscar.</param>
        /// <returns>La tarea correspondiente o 404 si no existe o no pertenece al usuario.</returns>
        [HttpGet("Tareas/{id}")]
        public IActionResult ObtenerTareaPorId(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_ObtenerTarea", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UsuarioID", ObtenerUsuarioId());

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var tarea = new Tarea
                {
                    Id = (int)reader["Id"],
                    Titulo = reader["Titulo"].ToString(),
                    Descripcion = reader["Descripcion"].ToString(),
                    Completado = (bool)reader["Completado"],
                    UsuarioId = (int)reader["UsuarioId"]
                };

                return Ok(tarea);
            }

            return NotFound();
        }

        /// <summary>
        /// Crea una nueva tarea asociada al usuario autenticado.
        /// </summary>
        /// <param name="registrarTareaDTO">Datos de la tarea a registrar.</param>
        /// <returns>Mensaje de confirmación si se creó exitosamente.</returns>
        [HttpPost("Tareas")]
        public IActionResult CrearTarea([FromBody] RegistrarTareaDTO registrarTareaDTO)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_RegistrarTarea", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Titulo", registrarTareaDTO.Titulo);
            command.Parameters.AddWithValue("@Descripcion", registrarTareaDTO.Descripcion);
            command.Parameters.AddWithValue("@Completado", registrarTareaDTO.Completado);
            command.Parameters.AddWithValue("@UsuarioId", ObtenerUsuarioId());

            connection.Open();
            command.ExecuteNonQuery();

            return Ok(new { mensaje = "Tarea registrada exitosamente" });
        }

        /// <summary>
        /// Actualiza una tarea existente si pertenece al usuario autenticado.
        /// </summary>
        /// <param name="id">ID de la tarea a actualizar.</param>
        /// <param name="registrarTareaDTO">Datos nuevos de la tarea.</param>
        /// <returns>Mensaje de éxito si se actualizó, o 404 si no se encontró.</returns>
        [HttpPut("Tareas/{id}")]
        public IActionResult ActualizarTarea(int id, [FromBody] RegistrarTareaDTO registrarTareaDTO)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_ActualizarTarea", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Titulo", registrarTareaDTO.Titulo);
            command.Parameters.AddWithValue("@Descripcion", registrarTareaDTO.Descripcion);
            command.Parameters.AddWithValue("@Completado", registrarTareaDTO.Completado);
            command.Parameters.AddWithValue("@UsuarioID", ObtenerUsuarioId());

            connection.Open();
            using var reader = command.ExecuteReader();
            int filas = 0;

            if (reader.Read())
                filas = Convert.ToInt32(reader["FilasAfectadas"]);

            if (filas > 0)
                return Ok(new { mensaje = "Tarea actualizada exitosamente" });

            return NotFound();
        }

        /// <summary>
        /// Elimina una tarea si pertenece al usuario autenticado.
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar.</param>
        /// <returns>Mensaje de éxito si se eliminó, o 404 si no se encontró.</returns>
        [HttpDelete("Tareas/{id}")]
        public IActionResult EliminarTarea(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_EliminarTarea", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UsuarioID", ObtenerUsuarioId());

            connection.Open();
            using var reader = command.ExecuteReader();
            int filas = 0;

            if (reader.Read())
                filas = Convert.ToInt32(reader["FilasAfectadas"]);

            if (filas > 0)
                return Ok(new { mensaje = "Tarea eliminada exitosamente" });

            return NotFound();
        }
    }
}
