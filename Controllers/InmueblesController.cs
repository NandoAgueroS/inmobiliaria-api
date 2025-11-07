using System.Data.Common;
using System.Security.Claims;
using System.Text.Json;
using InmobiliariaAPI.DTO;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

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
            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
            var inmuebles = await context.Inmuebles
              .Include(o => o.Propietario)
              .Where(x => x.IdPropietario == idPropietario && x.Estado == true).ToListAsync();
            return Ok(inmuebles);
        }

        [HttpPatch("disponible/{id}")]
        public async Task<IActionResult> ActualizarDisponible(int id)
        {

            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");

            if (id == 0)
            {
                return BadRequest("El id del inmueble es requerido");
            }
            else if (id != idPropietario)
            {
                return StatusCode(403, "No puede editar el inmueble de otro propietario");
            }

            Inmueble? inmuebleOriginal = await context.Inmuebles
              .SingleOrDefaultAsync(x => x.IdInmueble == id && x.IdPropietario == idPropietario);


            if (inmuebleOriginal == null)
            {
                return NotFound("No se encontró el inmueble");
            }
            inmuebleOriginal.Disponible = !inmuebleOriginal.Disponible;
            await context.SaveChangesAsync();

            return Ok(inmuebleOriginal);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Crear([FromForm] InmuebleImagenRequest inmuebleRequest)
        {
            int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
            Inmueble inmueble = new Inmueble(inmuebleRequest, idPropietario);
            IFormFile imagen = inmuebleRequest.Imagen;

            try
            {
                await context.Inmuebles.AddAsync(inmueble);
                await context.SaveChangesAsync();
                if (imagen == null)
                    return BadRequest();

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
                return Created();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "Error al guardar");
            }
        }
        [HttpGet("contrato-vigente")]
        public async Task<IActionResult> GetContratoVigente()
        {

            try
            {
                int idPropietario = int.Parse(User?.Identity?.Name ?? "0");
                var inmuebles = await context.Contratos
                  .Include(o => o.Inmueble)
                  .ThenInclude(o => o.Propietario)
                  .Include(o => o.Inmueble)
                  .ThenInclude(o => o.Tipo)
                  .Where(x => x.Inmueble.IdPropietario == idPropietario && x.FechaDesde < x.FechaHasta && x.FechaHasta > DateOnly.FromDateTime(DateTime.Now))
    .Select(x => x.Inmueble).ToListAsync();
                return Ok(inmuebles);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(503, "Error en la base de datos");
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
