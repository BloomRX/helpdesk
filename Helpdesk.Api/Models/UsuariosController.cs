using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.Controllers
{
    public class UsuariosController : Controller
    {
        // Simulando um banco de dados na memória
        private static List<Usuario> usuarios = new List<Usuario>();

        // GET: /Usuarios
        public IActionResult Index()
        {
            return View(usuarios);
        }

        // GET: /Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Usuarios/Create
        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            usuarios.Add(usuario);
            return RedirectToAction("Index");
        }
    }
}
