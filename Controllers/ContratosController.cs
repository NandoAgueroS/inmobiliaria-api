using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContratosController : ControllerBase
    {

        private readonly DataContext context;
        public ContratosController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet("inmueble/{idInmueble}")]
        public async Task<IActionResult> ListarPorInmueble([FromRoute] int idInmueble, [FromQuery] bool? vigentes)
        {

            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
            DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
            var contratosQuery = context.Contratos
              .Include(o => o.Inmueble)
              .ThenInclude(o => o.Propietario)
              .Include(o => o.Inquilino)
              .Where(x => x.Inmueble.IdPropietario == idPropietario && x.Inmueble.IdInmueble == idInmueble && x.Estado == true);
            if (vigentes != null)
            {
                if (vigentes.Value)
                    contratosQuery = contratosQuery.Where(x => x.FechaDesde < hoy && x.FechaHasta > hoy);
                else
                    contratosQuery = contratosQuery.Where(x => x.FechaDesde > hoy || x.FechaHasta < hoy);
            }
            var contratos = await contratosQuery.ToListAsync();
            return Ok(contratos);
        }

    }
}
