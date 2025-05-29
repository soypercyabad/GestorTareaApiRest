using GestorTareaApiRest.DTOs;
using GestorTareaApiRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace GestorTareaApiRest.Controllers
{
    [ApiController]
    [Route("/")]
    public class LoginController : Controller 
    {
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtService = new JwtService(configuration);
        }

        /// <summary>
        /// Loguear un usuario.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous] // No requiere token para acceder
        public IActionResult Login([FromBody] LoginUsuarioDTO loginUsuarioDTO)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("sp_Login", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Usuario", loginUsuarioDTO.NombreUsuario); // ✅ solo este

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                string claveGuardada = reader["ClaveSecreta"].ToString();

                if (!BCrypt.Net.BCrypt.Verify(loginUsuarioDTO.ClaveSecreta, claveGuardada))
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });

                int usuarioId = Convert.ToInt32(reader["Id"]);
                string token = _jwtService.GenerarToken(usuarioId, loginUsuarioDTO.NombreUsuario);
                return Ok(new { token });
            }

            return Unauthorized(new { mensaje = "Credenciales inválidas" });
        }
    }
}
