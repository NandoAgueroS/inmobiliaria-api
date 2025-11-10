using System.ComponentModel.DataAnnotations;


namespace InmobiliariaAPI.DTO
{

    public class ActualizarPropietarioRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(100)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El dni es obligatorio")]
        [MaxLength(30)]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El telefono es obligatorio")]
        [MaxLength(20)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [MaxLength(150)]
        public string Email { get; set; }
    }
}


