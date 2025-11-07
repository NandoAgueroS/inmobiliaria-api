using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.DTO;
using InmobiliariaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
            var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);
            if (propietario == null) return NotFound("No se encontró el propietario");
            return Ok(propietario);
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] Propietario propietario)
        {
            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

            if (propietario.IdPropietario != idPropietario)
                return StatusCode(403, "Solo puede editar su perfil");

            var propietarioOriginal = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);
            if (propietarioOriginal == null) return BadRequest("No se encontró el propietario");
            if (propietario.Clave == null)
            {
                propietario.Clave = propietarioOriginal.Clave;
            }
            else
            {

                string hashed = HashService.HashClave(configuration["Salt"] ?? "", propietario.Clave);
                propietario.Clave = hashed;
            }
            context.Entry(propietarioOriginal).CurrentValues.SetValues(propietario);
            context.SaveChanges();
            return Ok(propietario);
        }

        [HttpPatch("clave")]
        [AllowAnonymous]
        public async Task<IActionResult> CambiarClave([FromForm] CambiarClaveRequest cambiarClave)
        {

            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
            string nuevaHashed = HashService.HashClave(configuration["Salt"] ?? "", cambiarClave.Nueva);
            string viejaHashed = HashService.HashClave(configuration["Salt"] ?? "", cambiarClave.Actual);

            Propietario? propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

            if (propietario == null) return NotFound("No se encontró el propietario");

            if (viejaHashed != propietario.Clave) return BadRequest("Clave actual incorrecta");
            propietario.Clave = nuevaHashed;

            await context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest login)
        {
            try
            {
                string hashed = HashService.HashClave(configuration["Salt"] ?? "", login.Clave);
                var p = await context.Propietarios.FirstOrDefaultAsync(x => x.Email == login.Usuario && x.Estado == true);
                if (p == null || p.Clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var secreto = configuration["TokenAuthentication:SecretKey"];
                    if (string.IsNullOrEmpty(secreto))
                        throw new Exception("Falta configurar TokenAuthentication:Secret");
                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.IdPropietario.ToString()),
                        new Claim("FullName", p.Nombre + " " + p.Apellido),
                        new Claim(ClaimTypes.Role, "Propietario"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: configuration["TokenAuthentication:Issuer"],
                        audience: configuration["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
