using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Data;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Categorias
        public IActionResult Index()
        {
            return View(_context.Categorias.ToList());
        }

        // GET: /Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categorias/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);

            _context.Categorias.Add(categoria);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
