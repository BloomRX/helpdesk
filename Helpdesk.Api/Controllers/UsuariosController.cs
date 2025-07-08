using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using BCrypt.Net;
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
            // 1. Remove o erro de email existente antes de revalidar
            ModelState.Remove("Email");

            // 2. Verifica se o novo email já existe
            var emailExistente = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
            if (emailExistente != null)
            {
                ModelState.AddModelError("Email", "Este email já está em uso");
            }

            // 3. Validação manual da confirmação de senha
            if (usuario.Senha != usuario.ConfirmarSenha)
            {
                ModelState.AddModelError("ConfirmarSenha", "As senhas não coincidem");
            }

            if (ModelState.IsValid)
            {
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                usuario.ConfirmarSenha = null;
                usuario.Tipo = TipoUsuario.Membro;
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
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                ViewBag.Erro = "Email ou senha inválidos";
                return View();
            }

            if (usuario != null)
            {
                // Força temporariamente esse e-mail como admin
                if (usuario.Email == "lucasbarbano71@gmail.com")
                {
                    usuario.Tipo = TipoUsuario.Admin;
                    _context.SaveChanges();
                }

                HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
                HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString()); // "Admin" ou "Membro"
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuarios");
        }

    }
}
