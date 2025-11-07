using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.DTO
{
    public class InmuebleImagenRequest
    {
        [Required]
        public string Direccion { get; set; }

        [Required]
        public string Uso { get; set; }

        [Required]
        public int? IdTipo { get; set; }

        [Required]
        public int? Ambientes { get; set; }

        [Required]
        public int? Superficie { get; set; }

        [Required]
        public decimal? Valor { get; set; }

        [Required]
        public decimal? Latitud { get; set; }

        [Required]
        public decimal? Longitud { get; set; }

        [Required]
        public IFormFile Imagen { get; set; }
    }
}
