using System.ComponentModel.DataAnnotations;

namespace examenwed3.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1, 10000)]
        [Display(Name = "Precio por Noche")]
        public decimal PrecioPorNoche { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        // Relación: Un hotel puede tener muchas reservas
        public virtual ICollection<Reserva>? Reservas { get; set; }
    }
}