using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.DTO;
using InmobiliariaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;

namespace InmobiliariaAPI
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropietariosController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;

        public PropietariosController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return NotFound("No se encontró el propietario");

                return Ok(propietario);
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(503, "Error en la base de datos");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Error en el servidor");
            }
        }

        [HttpPut("perfil")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarPropietarioRequest actualizarPropietario)
        {
            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

                var propietarioOriginal = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietarioOriginal == null) return BadRequest("No se encontró el propietario");

                propietarioOriginal.Nombre = actualizarPropietario.Nombre;
                propietarioOriginal.Apellido = actualizarPropietario.Apellido;
                propietarioOriginal.Dni = actualizarPropietario.Dni;
                propietarioOriginal.Email = actualizarPropietario.Email;
                propietarioOriginal.Telefono = actualizarPropietario.Telefono;

                context.SaveChanges();
                return Ok(propietarioOriginal);
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(503, "Error en la base de datos");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Error en el servidor");
            }
        }

        [HttpPatch("clave")]
        public async Task<IActionResult> CambiarClave([FromForm] CambiarClaveRequest cambiarClave)
        {

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");

                string nuevaHashed = HashService.HashClave(configuration["Salt"] ?? "", cambiarClave.Nueva);
                string viejaHashed = HashService.HashClave(configuration["Salt"] ?? "", cambiarClave.Actual);

                if (propietario == null) return NotFound("No se encontró el propietario");

                if (viejaHashed != propietario.Clave) return BadRequest("Clave actual incorrecta");
                propietario.Clave = nuevaHashed;

                await context.SaveChangesAsync();

                return Ok();
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(503, "Error en la base de datos");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Error en el servidor");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest login)
        {
            try
            {
                string hashed = HashService.HashClave(configuration["Salt"] ?? "", login.Clave);

                var propietario = await context.Propietarios.FirstOrDefaultAsync(x => x.Email == login.Usuario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");

                if (propietario == null || propietario.Clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    JwtSecurityToken token = TokenService.CrearToken(configuration, propietario);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(503, "Error en la base de datos");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Error en el servidor");
            }
        }
    }
}
