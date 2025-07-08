using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BCrypt.Net;

namespace Helpdesk.Api.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN - GET
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN - POST
        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                ViewBag.Erro = "Email ou senha inválidos";
                return View();
            }

            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            return RedirectToAction("Index");
        }

        // CADASTRO - GET
        public IActionResult Create()
        {
            return View();
        }

        // CADASTRO - POST
        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "Email já em uso");
            }

            if (ModelState.IsValid)
            {
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                usuario.Tipo = "Membro";

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(usuario);
        }

        // LISTAGEM (apenas se logado)
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UsuarioEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
