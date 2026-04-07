using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace examenwed3.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        // Relación: Un usuario puede tener muchas reservas
        public virtual ICollection<Reserva>? Reservas { get; set; }
    }
}