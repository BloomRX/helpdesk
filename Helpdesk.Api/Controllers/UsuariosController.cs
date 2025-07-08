using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                // Se o email for o do admin, define como Admin
                usuario.Tipo = usuario.Email.ToLower() == "admin@admin.com" ? "Admin" : "Membro";

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(usuario);
        }

        // LISTAGEM (apenas se for admin)
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UsuarioEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null || usuario.Tipo != "Admin")
                //return Unauthorized();
                return RedirectToAction("Login");

            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult TornarAdmin(int id)
        {
            var emailLogado = HttpContext.Session.GetString("UsuarioEmail");
            var logado = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);

            if (logado == null || logado.Tipo != "Admin")
                return Unauthorized();

            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                usuario.Tipo = "Admin";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
