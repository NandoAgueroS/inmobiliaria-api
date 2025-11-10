using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.DTO
{
    public class InmuebleImagenRequest
    {
        [Required(ErrorMessage = "La direccion es obligaroria")]
        [MaxLength(250)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El uso es obligarorio")]
        [MaxLength(11)]
        public string Uso { get; set; }

        [Required(ErrorMessage = "El tipo es obligarorio")]
        public int? IdTipo { get; set; }

        [Required(ErrorMessage = "La cantidad de ambientes es obligaroria")]
        public int? Ambientes { get; set; }

        [Required(ErrorMessage = "La superficie es obligaroria")]
        public int? Superficie { get; set; }

        [Required(ErrorMessage = "El valor es obligarorio")]
        public decimal? Valor { get; set; }

        [Required(ErrorMessage = "La latitud es obligaroria")]
        public decimal? Latitud { get; set; }

        [Required(ErrorMessage = "La longitud es obligaroria")]
        public decimal? Longitud { get; set; }

        [Required(ErrorMessage = "La imagen es obligaroria")]
        public IFormFile Imagen { get; set; }
    }
}
