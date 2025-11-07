using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.DTO
{
    public class LoginRequest
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Clave { get; set; }
    }
}
