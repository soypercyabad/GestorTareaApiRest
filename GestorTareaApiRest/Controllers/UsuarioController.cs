using GestorTareaApiRest.DTOs;
using GestorTareaApiRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using GestorTareaApiRest.Models;

namespace GestorTareaApiRest.Controllers
{
    [ApiController]
    [Route("/")]
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public UsuarioController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtService = new JwtService(configuration);
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        [HttpPost("usuarios")]
        public IActionResult RegistrarUsuario([FromBody] RegistrarUsuarioDTO registrarUsuarioDTO)
        {
            string claveSecretaHasheada = BCrypt.Net.BCrypt.HashPassword(registrarUsuarioDTO.ClaveSecreta);
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_RegistrarUsuario", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Nombres", registrarUsuarioDTO.Nombres);
            command.Parameters.AddWithValue("@ApellidoPaterno", registrarUsuarioDTO.ApellidoPaterno);
            command.Parameters.AddWithValue("@ApellidoMaterno", registrarUsuarioDTO.ApellidoMaterno);
            command.Parameters.AddWithValue("@Usuario", registrarUsuarioDTO.NombreUsuario);
            command.Parameters.AddWithValue("@ClaveSecreta", claveSecretaHasheada);

            connection.Open();
            command.ExecuteNonQuery();

            return Ok(new { mensaje = "Usuario registrado exitosamente" });
        }

        /// <summary>
        /// Obtiene las tareas del usuario autenticado, con opción de filtrar por estado y paginación.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="completado">Filtra por estado de la tarea (true = completadas, false = pendientes)</param>
        /// <param name="page">Número de página para paginación</param>
        /// <param name="limit">Cantidad de tareas por página</param>
        /// <returns>Lista de tareas del usuario</returns>

        [HttpGet("/usuarios/{id}/tareas")]
        [Authorize]
        public IActionResult ObtenerTareasPorUsuario(int id, [FromQuery] bool? completado, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var usuarioIdToken = int.Parse(User.FindFirst("usuarioId")?.Value ?? "0");
            if (usuarioIdToken != id)
                return Forbid();

            var tareas = new List<Tarea>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_ListarTareasUsuario", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UsuarioId", id);
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


    }
}