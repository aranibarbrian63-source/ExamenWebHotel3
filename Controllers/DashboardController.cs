using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using examenwed3.Data;
using System.Linq;

namespace examenwed3.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly examenwed3Context _context;
        private readonly ApplicationDbContext _identityContext;

        public DashboardController(examenwed3Context context, ApplicationDbContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        public IActionResult Index()
        {
            // Contamos hoteles y reservas del contexto de negocio
            ViewBag.TotalHoteles = _context.Hotel.Count();
            ViewBag.TotalReservas = _context.Reserva.Count();

            // Contamos usuarios del contexto de Identity para evitar el error CS1061
            ViewBag.TotalUsuarios = _identityContext.Users.Count();

            return View();
        }
    }
}