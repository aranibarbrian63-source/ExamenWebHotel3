using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using examenwed3.Data;
using examenwed3.Models;

namespace examenwed3.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📊 DASHBOARD: Total de hoteles, usuarios y reservas
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalHoteles = await _context.Hoteles.CountAsync();
            ViewBag.TotalUsuarios = await _context.Users.CountAsync();
            ViewBag.TotalReservas = await _context.Reservas.CountAsync();

            var reservasPorHotel = await _context.Reservas
                .Include(r => r.Hotel)
                .GroupBy(r => r.Hotel.Nombre)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return View(reservasPorHotel);
        }

        // 📄 REPORTE: Lista de reservas para generar PDF
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Reporte()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Hotel)
                .Include(r => r.Usuario)
                .ToListAsync();
            return View(reservas);
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrador"))
            {
                var reservas = _context.Reservas.Include(r => r.Hotel).Include(r => r.Usuario);
                return View(await reservas.ToListAsync());
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var reservas = _context.Reservas
                    .Include(r => r.Hotel)
                    .Where(r => r.UsuarioId == userId);
                return View(await reservas.ToListAsync());
            }
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewBag.HotelId = new SelectList(_context.Hoteles, "Id", "Nombre");

            // Si es Admin puede elegir usuario, si es Cliente no necesita ver el dropdown
            if (User.IsInRole("Administrador"))
            {
                ViewBag.UsuarioId = new SelectList(_context.Users, "Id", "Email");
            }
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaInicio,FechaFin,HotelId,UsuarioId")] Reserva reserva)
        {
            // --- ASIGNACIÓN AUTOMÁTICA ---
            // Si no es admin, forzamos que el UsuarioId sea el del usuario que tiene la sesión iniciada
            if (!User.IsInRole("Administrador"))
            {
                reserva.UsuarioId = _userManager.GetUserId(User);
                // Quitamos el error de validación de UsuarioId si existiera, ya que lo asignamos manualmente
                ModelState.Remove("UsuarioId");
            }

            // VALIDACIONES DE NEGOCIO
            if (reserva.FechaInicio < DateTime.Today)
            {
                ModelState.AddModelError("FechaInicio", "No se permiten reservas en fechas pasadas.");
            }

            if (reserva.FechaFin <= reserva.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser mayor a la de inicio.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si hay error, recargamos los ViewBags
            ViewBag.HotelId = new SelectList(_context.Hoteles, "Id", "Nombre", reserva.HotelId);
            if (User.IsInRole("Administrador"))
            {
                ViewBag.UsuarioId = new SelectList(_context.Users, "Id", "Email", reserva.UsuarioId);
            }
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            ViewBag.HotelId = new SelectList(_context.Hoteles, "Id", "Nombre", reserva.HotelId);
            ViewBag.UsuarioId = new SelectList(_context.Users, "Id", "Email", reserva.UsuarioId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaInicio,FechaFin,HotelId,UsuarioId")] Reserva reserva)
        {
            if (id != reserva.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.HotelId = new SelectList(_context.Hoteles, "Id", "Nombre", reserva.HotelId);
            ViewBag.UsuarioId = new SelectList(_context.Users, "Id", "Email", reserva.UsuarioId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas
                .Include(r => r.Hotel)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null) _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}