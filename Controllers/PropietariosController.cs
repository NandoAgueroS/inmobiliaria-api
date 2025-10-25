using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        public IActionResult GetPropietarios()
        {
            int idPropietario = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var propietarios = context.Propietarios.Where(x => x.IdPropietario == idPropietario).FirstOrDefault();
            return Ok(propietarios);
        }

        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] Propietario propietario)
        {
            Propietario propietarioOriginal = context.Propietarios.Where(x => x.IdPropietario == propietario.IdPropietario).FirstOrDefault();
            if (propietario.Clave == null)
            {
                propietario.Clave = propietarioOriginal.Clave;
            }
            else
            {

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: propietario.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                propietario.Clave = hashed;
            }
            context.Entry(propietarioOriginal).CurrentValues.SetValues(propietario);
            context.SaveChanges();
            return Ok(propietario);
        }

        [HttpPost("reset")]
        [AllowAnonymous]
        public IActionResult Reset([FromForm] LoginDTO loginDTO)
        {

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginDTO.Clave,
                salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            context.Propietarios.Where(x => x.Email == loginDTO.Usuario).FirstOrDefault().Clave = hashed;
            context.SaveChanges();
            return Ok();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginDTO.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = await context.Propietarios.FirstOrDefaultAsync(x => x.Email == loginDTO.Usuario);
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
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim(ClaimTypes.NameIdentifier, p.IdPropietario.ToString()),
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
