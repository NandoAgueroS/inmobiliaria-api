using System.Data.Common;
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
        public async Task<IActionResult> ListarPorInmueble([FromRoute] int idInmueble)
        {

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");

                DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);

                var contratos = await context.Contratos
                  .Include(o => o.Inmueble)
                  .ThenInclude(o => o.Propietario)
                  .Include(o => o.Inquilino)
                  .Where(x => x.Inmueble.IdPropietario == idPropietario && x.Inmueble.IdInmueble == idInmueble && x.Estado == true).ToListAsync();

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
