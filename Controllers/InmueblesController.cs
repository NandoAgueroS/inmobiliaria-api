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
    public class InmueblesController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;
        public InmueblesController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Inmuebles()
        {
            int idPropietario = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var inmuebles = context.Inmuebles.Where(x => x.IdPropietario == idPropietario).ToList();
            return Ok(inmuebles);
        }

        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] Inmueble inmueble)
        {
            Inmueble inmuebleOriginal = context.Inmuebles.Where(x => x.IdInmueble == inmueble.IdInmueble.Value).FirstOrDefault();
            inmuebleOriginal.Disponible = inmueble.Disponible;
            context.SaveChanges();
            return Ok(inmuebleOriginal);
        }

    }
}
