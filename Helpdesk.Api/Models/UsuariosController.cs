using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using Microsoft.AspNetCore.Http;

namespace Helpdesk.Api.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Verifica se o usuário está logado e é Admin
            var tipoUsuario = HttpContext.Session.GetString("UsuarioTipo");
            if (tipoUsuario != TipoUsuario.Admin.ToString())
            {
                // Pode redirecionar para login, página de erro ou retornar Unauthorized
                return Unauthorized(); // ou return RedirectToAction("Login", "Usuarios");
            }

            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Tipo = TipoUsuario.Membro; // Força novo usuário como Membro
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Login", "Usuarios");
            }


            return View(usuario);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult TornarAdmin(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            if (HttpContext.Session.GetString("UsuarioTipo") != TipoUsuario.Admin.ToString())
                return Unauthorized();

            usuario.Tipo = TipoUsuario.Admin;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
                HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString()); // "Admin" ou "Membro"
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Email ou senha inválidos";
            return View();
        }

    }
}
