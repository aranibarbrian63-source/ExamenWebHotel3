using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace examenwed3.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")] // Verifica que diga Required con R mayúscula
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty; // El "= string.Empty" quita la advertencia CS8618

        public virtual ICollection<Reserva>? Reservas { get; set; }
        
    }
}