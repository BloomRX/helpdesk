using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;

namespace Helpdesk.Api.Controllers
{
    [Authorize] // Requer login para acessar qualquer ação
    public class SolicitacoesController : Controller
    {
        private readonly AppDbContext _context;

        public SolicitacoesController(AppDbContext context)
        {
            _context = context;
        }

        // Listar todas as solicitações
        [AllowAnonymous]
        public IActionResult Index()
        {
            var solicitacoes = _context.Solicitacoes
                .Include(s => s.Respostas)
                .ToList();

            return View(solicitacoes);
        }

        // Visualizar os detalhes (e respostas)
        [AllowAnonymous]
        public IActionResult Detalhes(int id)
        {
            var solicitacao = _context.Solicitacoes
                .Include(s => s.Respostas)
                .FirstOrDefault(s => s.Id == id);

            if (solicitacao == null) return NotFound();

            return View(solicitacao);
        }

        // Formulário para criar nova solicitação
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Criar nova solicitação (só logado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Solicitacao solicitacao)
        {
            if (ModelState.IsValid)
            {
                solicitacao.EmailUsuario = User.Identity?.Name!;
                solicitacao.DataAbertura = DateTime.Now;

                _context.Solicitacoes.Add(solicitacao);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(solicitacao);
        }

        // Formulário de edição
        public IActionResult Editar(int id)
        {
            var solicitacao = _context.Solicitacoes.Find(id);
            if (solicitacao == null) return NotFound();

            if (!UsuarioPodeAlterar(solicitacao))
                return Forbid();

            return View(solicitacao);
        }

        // Atualiza solicitação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Solicitacao solicitacao)
        {
            var original = _context.Solicitacoes.AsNoTracking().FirstOrDefault(s => s.Id == solicitacao.Id);
            if (original == null) return NotFound();

            if (!UsuarioPodeAlterar(original))
                return Forbid();

            solicitacao.EmailUsuario = original.EmailUsuario; // mantém o autor original
            solicitacao.DataAbertura = original.DataAbertura;

            _context.Solicitacoes.Update(solicitacao);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Excluir solicitação
        public IActionResult Excluir(int id)
        {
            var solicitacao = _context.Solicitacoes.Find(id);
            if (solicitacao == null) return NotFound();

            if (!UsuarioPodeAlterar(solicitacao))
                return Forbid();

            _context.Solicitacoes.Remove(solicitacao);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Responder a uma solicitação (apenas logado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Responder(int solicitacaoId, string conteudo)
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Usuarios");

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

        // Verifica se o usuário atual é o autor da solicitação ou admin
        private bool UsuarioPodeAlterar(Solicitacao solicitacao)
        {
            return User.IsInRole("Admin") || solicitacao.EmailUsuario == User.Identity?.Name;
        }
    }
}
