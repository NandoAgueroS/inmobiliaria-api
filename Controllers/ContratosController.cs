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
        public async Task<IActionResult> ListarPorInmueble([FromRoute] int idInmueble, [FromQuery] bool? vigentes)
        {

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");
                DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
                var contratosQuery = context.Contratos
                  .Include(o => o.Inmueble)
                  .ThenInclude(o => o.Propietario)
                  .Include(o => o.Inquilino)
                  .Where(x => x.Inmueble.IdPropietario == idPropietario && x.Inmueble.IdInmueble == idInmueble && x.Estado == true);
                if (vigentes != null)
                {
                    if (vigentes.Value)
                        contratosQuery = contratosQuery.Where(x => x.FechaDesde <= hoy && (x.FechaHasta >= hoy && x.FechaAnulado == null) || (x.FechaAnulado != null && x.FechaAnulado >= hoy));
                    else
                        contratosQuery = contratosQuery.Where(x => x.FechaDesde >= hoy || (x.FechaHasta <= hoy && x.FechaAnulado == null) || (x.FechaAnulado != null && x.FechaAnulado <= hoy));
                }
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
