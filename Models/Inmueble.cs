using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{

    public class Inmueble
    {

        [Key]
        public int? IdInmueble { get; set; }

        public string? Direccion { get; set; }

        public string? Uso { get; set; }

        public int IdTipo { get; set; }

        [ForeignKey("IdTipo")]
        public Tipo? Tipo { get; set; }

        public int Ambientes { get; set; }

        public int Superficie { get; set; }

        public decimal Valor { get; set; }

        public string? Imagen { get; set; }

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        [ForeignKey("IdPropietario")]
        public int IdPropietario { get; set; }

        [ForeignKey("IdPropietario")]
        public Propietario? Propietario { get; set; }

        public bool Disponible { get; set; }

        public bool TieneContratoVigente { get; set; }

        public bool Estado { get; set; }




    }
}
