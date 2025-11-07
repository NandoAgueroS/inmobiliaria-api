using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.DTO
{
    public class CambiarClaveRequest
    {
        [Required]
        public string Actual { get; set; }
        [Required]
        public string Nueva { get; set; }
    }
}
