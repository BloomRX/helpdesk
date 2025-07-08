using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Helpdesk.Api.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BcryptSettings _bcryptSettings;
        private const string AdminEmail = "lucasbarbano71@gmail.com"; // Seu e-mail como admin

        public UsuariosController(
            AppDbContext context,
            IOptions<BcryptSettings> bcryptSettings)
        {
            _context = context;
            _bcryptSettings = bcryptSettings.Value;
        }

        public IActionResult Index()
        {
            // Verifica se o usuário está logado e é Admin
            var tipoUsuario = HttpContext.Session.GetString("UsuarioTipo");
            if (tipoUsuario != TipoUsuario.Admin.ToString())
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            ModelState.Remove("Email");
            
            // Verifica email duplicado
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "Este email já está em uso");
            }

            // Valida confirmação de senha
            if (usuario.Senha != usuario.ConfirmarSenha)
            {
                ModelState.AddModelError("ConfirmarSenha", "As senhas não coincidem");
            }

            if (ModelState.IsValid)
            {
                // Define o tipo de usuário (seu email sempre será admin)
                usuario.Tipo = usuario.Email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase) 
                    ? TipoUsuario.Admin 
                    : TipoUsuario.Membro;

                // Hash da senha com BCrypt
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(
                    usuario.Senha, 
                    _bcryptSettings.WorkFactor);
                
                usuario.ConfirmarSenha = null; // Limpa campo não mapeado
                
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cadastro realizado com sucesso!";
                
                // Se for admin, já faz login automaticamente
                if (usuario.Tipo == TipoUsuario.Admin)
                {
                    HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
                    HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString());
                    return RedirectToAction("Index", "Home");
                }
                
                return RedirectToAction("Login");
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Verifica se usuário atual é admin
            if (HttpContext.Session.GetString("UsuarioTipo") != TipoUsuario.Admin.ToString())
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Impede a exclusão do próprio admin
            if (usuario.Email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Não é possível remover o administrador principal";
                return RedirectToAction("Index");
            }

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usuário removido com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TornarAdmin(int id)
        {
            // Verifica se usuário atual é admin
            if (HttpContext.Session.GetString("UsuarioTipo") != TipoUsuario.Admin.ToString())
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Tipo = TipoUsuario.Admin;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usuário promovido a administrador!";
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            // Se já estiver logado, redireciona
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioEmail")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            // Verificação segura com BCrypt
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                ViewBag.ErrorMessage = "Email ou senha inválidos";
                return View();
            }

            // Garante que seu e-mail sempre será admin
            if (usuario.Email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                usuario.Tipo = TipoUsuario.Admin;
                _context.SaveChanges();
            }

            // Configura sessão
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString());

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Logout realizado com sucesso!";
            return RedirectToAction("Login");
        }
    }

    public class BcryptSettings
    {
        public int WorkFactor { get; set; } = 12;
    }
}