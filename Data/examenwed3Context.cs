using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using examenwed3.Models;

namespace examenwed3.Data
{
    public class examenwed3Context : DbContext
    {
        public examenwed3Context (DbContextOptions<examenwed3Context> options)
            : base(options)
        {
        }

        public DbSet<examenwed3.Models.Hotel> Hotel { get; set; } = default!;
        public DbSet<examenwed3.Models.Reserva> Reserva { get; set; } = default!;
    }
}
