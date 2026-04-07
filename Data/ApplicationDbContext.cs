using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using examenwed3.Models;

namespace examenwed3.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hoteles { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Importante: No borrar el base.OnModelCreating para Identity
            base.OnModelCreating(builder);

            // 1. Configuración de Precisión para el Precio
            builder.Entity<Hotel>()
                .Property(h => h.PrecioPorNoche)
                .HasPrecision(18, 2);

            // 2. Relación: Un Hotel tiene muchas Reservas
            builder.Entity<Reserva>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el hotel, se borran sus reservas

            // 3. Relación: Un Usuario (Identity) tiene muchas Reservas
            builder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}