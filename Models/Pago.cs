using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{

    public class Pago
    {
        [Key]
        public int? IdPago { get; set; }

        [Required(ErrorMessage = "Numero de pago es un campo obligatorio")]
        public string NumeroPago { get; set; }

        [Required(ErrorMessage = "El concepto es un campo obligatorio")]
        public string Concepto { get; set; }

        [Required(ErrorMessage = "El monto es un campo obligatorio")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El Fecha es un campo obligatorio")]
        public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required(ErrorMessage = "El Contrato es un campo obligatorio")]
        public int IdContrato { get; set; }

        [ForeignKey("IdContrato")]
        public Contrato? Contrato { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? CorrespondeAMes { get; set; }

        public int CreadoPor { get; set; }

        public int? AnuladoPor { get; set; }

        public bool Estado { get; set; }


    }
}
