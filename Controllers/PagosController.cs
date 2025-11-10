using System.Data.Common;
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

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return NotFound("No se encontró el propietario");

                var contratosQuery = context.Pagos
                  .Include(o => o.Contrato)
                  .ThenInclude(o => o.Inmueble)
                  .ThenInclude(o => o.Propietario)
                  .Where(x => x.Contrato.Inmueble.IdPropietario == idPropietario && x.Contrato.IdContrato == idContrato && x.Estado == true);

                var contratos = await contratosQuery.ToListAsync();

                return Ok(contratos);
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
