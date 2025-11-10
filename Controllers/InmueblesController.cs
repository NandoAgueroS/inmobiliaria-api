using System.Data.Common;
using System.Security.Claims;
using System.Text.Json;
using InmobiliariaAPI.DTO;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InmueblesController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webEnv;

        public InmueblesController(DataContext context, IConfiguration configuration, IWebHostEnvironment webEnv)
        {
            this.context = context;
            this.configuration = configuration;
            this.webEnv = webEnv;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);
                if (propietario == null) return BadRequest("No se encontró el propietario");

                var inmuebles = await context.Inmuebles
                  .Include(o => o.Propietario)
                  .Include(o => o.Tipo)
                  .Where(x => x.IdPropietario == idPropietario && x.Estado == true).ToListAsync();

                return Ok(inmuebles);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar([FromRoute] int id)
        {
            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");

                var inmueble = await context.Inmuebles
                  .Include(o => o.Propietario)
                  .Include(o => o.Tipo)
                  .SingleOrDefaultAsync(x => x.IdInmueble == id && x.Estado == true);

                if (inmueble == null) return NotFound("No se encontró el inmueble");
                if (inmueble.IdPropietario != idPropietario) return BadRequest("Solo puede ver sus inmuebles");

                return Ok(inmueble);
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

        [HttpPatch("disponible/{id}")]
        public async Task<IActionResult> ActualizarDisponible(int id)
        {

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var propietario = await context.Propietarios.SingleOrDefaultAsync(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");

                if (id == 0)
                {
                    return BadRequest("El id del inmueble es requerido");
                }

                Inmueble? inmuebleOriginal = await context.Inmuebles
                  .SingleOrDefaultAsync(x => x.IdInmueble == id && x.IdPropietario == idPropietario && x.Estado == true);


                if (inmuebleOriginal == null)
                {
                    return NotFound("No se encontró el inmueble");
                }
                inmuebleOriginal.Disponible = !inmuebleOriginal.Disponible;
                await context.SaveChangesAsync();

                return Ok(inmuebleOriginal);
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

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Crear([FromForm] InmuebleImagenRequest inmuebleRequest)
        {
            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

                var propietario = context.Propietarios.SingleOrDefault(x => x.IdPropietario == idPropietario && x.Estado == true);

                if (propietario == null) return BadRequest("No se encontró el propietario");

                Inmueble inmueble = new Inmueble(inmuebleRequest, idPropietario);
                IFormFile imagen = inmuebleRequest.Imagen;

                await context.Inmuebles.AddAsync(inmueble);
                await context.SaveChangesAsync();

                string wwwPath = webEnv.WebRootPath;
                string path = Path.Combine(wwwPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = "inmueble_" + inmueble.IdInmueble + Path.GetExtension(imagen.FileName);
                string pathCompleto = Path.Combine(path, fileName);
                inmueble.Imagen = Path.Combine("/Uploads", fileName);
                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }

                await context.SaveChangesAsync();
                return StatusCode(201, inmueble);
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

        [HttpGet("contrato-vigente")]
        public async Task<IActionResult> GetContratoVigente()
        {

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

                DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);

                var inmuebles = await context.Contratos
                  .Include(o => o.Inmueble)
                  .ThenInclude(o => o.Propietario)
                  .Include(o => o.Inmueble)
                  .ThenInclude(o => o.Tipo)
                  .Where(x => x.FechaDesde <= hoy && (x.FechaHasta >= hoy && x.FechaAnulado == null) || (x.FechaAnulado != null && x.FechaAnulado >= hoy))
                  .Select(x => x.Inmueble).ToListAsync();

                return Ok(inmuebles);
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
