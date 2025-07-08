using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Api.Controllers
{
    [Authorize] // Exige login para todas as ações (exceto onde indicado)
    public class SolicitacoesController : Controller
    {
        private readonly AppDbContext _context;

        public SolicitacoesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Solicitacao solicitacao)
        {
            if (ModelState.IsValid)
            {
                _context.Solicitacoes.Add(solicitacao);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(solicitacao);
        }
        public IActionResult Index()
        {
            var solicitacoes = _context.Solicitacoes.ToList();
            return View(solicitacoes);
        }

        public IActionResult Detalhes(int id)
        {
            var solicitacao = _context.Solicitacoes
                .Include(s => s.Respostas)
                .FirstOrDefault(s => s.Id == id);

            if (solicitacao == null) return NotFound();

            return View(solicitacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Responder(int solicitacaoId, string conteudo)
        {
            var email = HttpContext.Session.GetString("UsuarioEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Usuarios");

            var resposta = new Resposta
            {
                Conteudo = conteudo,
                EmailUsuario = email,
                SolicitacaoId = solicitacaoId
            };

            _context.Respostas.Add(resposta);
            _context.SaveChanges();

            return RedirectToAction("Detalhes", new { id = solicitacaoId });
        }
    }
}
