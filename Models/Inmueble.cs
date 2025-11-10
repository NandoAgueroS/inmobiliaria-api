using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InmobiliariaAPI.DTO;

namespace InmobiliariaAPI.Models
{

    public class Inmueble
    {

        [Key]
        public int? IdInmueble { get; set; }

        [MaxLength(250)]
        public string Direccion { get; set; }

        [MaxLength(11)]
        public string Uso { get; set; }

        public int IdTipo { get; set; }

        [ForeignKey("IdTipo")]
        public Tipo? Tipo { get; set; }

        public int Ambientes { get; set; }

        public int Superficie { get; set; }

        public decimal Valor { get; set; }

        public string? Imagen { get; set; } = "";

        public decimal Latitud { get; set; }

        public decimal Longitud { get; set; }

        public int IdPropietario { get; set; }

        [ForeignKey("IdPropietario")]
        public Propietario? Propietario { get; set; }

        public bool? Disponible { get; set; } = false;

        public bool? Estado { get; set; } = true;

        public Inmueble() { }

        public Inmueble(InmuebleImagenRequest inmuebleImagenRequest, int idPropietario)
        {

            Direccion = inmuebleImagenRequest.Direccion;
            Uso = inmuebleImagenRequest.Uso;
            IdTipo = inmuebleImagenRequest.IdTipo.Value;
            Ambientes = inmuebleImagenRequest.Ambientes.Value;
            Superficie = inmuebleImagenRequest.Superficie.Value;
            Valor = inmuebleImagenRequest.Valor.Value;
            Latitud = inmuebleImagenRequest.Latitud.Value;
            Longitud = inmuebleImagenRequest.Longitud.Value;
            IdPropietario = idPropietario;
        }
    }
}
