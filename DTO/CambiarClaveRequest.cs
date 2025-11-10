using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.DTO
{
    public class CambiarClaveRequest
    {
        [Required(ErrorMessage = "La clave actual es obligatoria")]
        public string Actual { get; set; }
        [Required(ErrorMessage = "La clave nueva es obligatoria")]
        public string Nueva { get; set; }
    }
}
