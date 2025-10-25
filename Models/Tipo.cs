using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models
{
    public class Tipo
    {
        [Key]
        public int IdTipo { get; set; }

        [Required(ErrorMessage = "La descripcion es un campo obligatorio")]
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
