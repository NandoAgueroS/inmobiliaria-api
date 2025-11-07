using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagosController : ControllerBase
    {

        private readonly DataContext context;
        public PagosController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet("contrato/{idContrato}")]
        public async Task<IActionResult> ListarPorContrato([FromRoute] int idContrato, [FromQuery] bool? anulados)
        {

            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
            var contratosQuery = context.Pagos
              .Include(o => o.Contrato)
              .ThenInclude(o => o.Inmueble)
              .ThenInclude(o => o.Propietario)
              .Where(x => x.Contrato.Inmueble.IdPropietario == idPropietario && x.Contrato.IdContrato == idContrato && x.Estado == true);
            var contratos = await contratosQuery.ToListAsync();
            return Ok(contratos);
        }
    }
}
