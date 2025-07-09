using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Api.Data;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helpdesk.Api.Controllers
{
    [Authorize]
    public class SolicitacoesController : Controller
    {
        private readonly AppDbContext _context;
        public SolicitacoesController(AppDbContext ctx) => _context = ctx;

        // GET: /Solicitacoes
        [AllowAnonymous]
        public IActionResult Index(int? categoriaId, StatusSolicitacao? status, string colaborador)
        {
            var q = _context.Solicitacoes
                .Include(s => s.Categoria)
                .Include(s => s.Respostas)
                .AsQueryable();

            if (categoriaId.HasValue)
                q = q.Where(s => s.CategoriaId == categoriaId.Value);

            if (status.HasValue)
                q = q.Where(s => s.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(colaborador))
                q = q.Where(s => s.EmailUsuario == colaborador);

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.StatusList = Enum.GetValues<StatusSolicitacao>();
            return View(q.OrderByDescending(s => s.DataAbertura).ToList());
        }

        // GET: /Solicitacoes/Details/5
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var sol = _context.Solicitacoes
                .Include(s => s.Categoria)
                .Include(s => s.Respostas)
                .FirstOrDefault(s => s.Id == id);
            if (sol == null) return NotFound();
            return View(sol);
        }

        // GET: /Solicitacoes/Create
        public IActionResult Create()
        {
            CarregarCategorias();
            return View(new Solicitacao
            {
                DataAbertura = DateTime.Now,
                EmailUsuario = User.Identity!.Name!
            });
        }

        // POST: /Solicitacoes/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Solicitacao solicitacao)
        {
            if (!ModelState.IsValid)
            {
                CarregarCategorias();
                return View(solicitacao);
            }

            solicitacao.EmailUsuario = User.Identity!.Name!;
            solicitacao.DataAbertura = DateTime.Now;
            _context.Solicitacoes.Add(solicitacao);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Solicitacoes/Edit/5
        public IActionResult Edit(int id)
        {
            var sol = _context.Solicitacoes.Find(id);
            if (sol == null) return NotFound();
            if (!UsuarioPodeAlterar(sol)) return Forbid();

            CarregarCategorias(sol.CategoriaId);
            return View(sol);
        }

        // POST: /Solicitacoes/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Solicitacao form)
        {
            if (id != form.Id) return BadRequest();
            var orig = _context.Solicitacoes.AsNoTracking().FirstOrDefault(s => s.Id == id);
            if (orig == null) return NotFound();
            if (!UsuarioPodeAlterar(orig)) return Forbid();

            orig.Titulo      = form.Titulo;
            orig.Descricao   = form.Descricao;
            orig.CategoriaId = form.CategoriaId;
            orig.Status      = form.Status;

            _context.Solicitacoes.Update(orig);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Solicitacoes/Delete/5
        public IActionResult Delete(int id)
        {
            var sol = _context.Solicitacoes.Find(id);
            if (sol == null) return NotFound();
            if (!UsuarioPodeAlterar(sol)) return Forbid();
            return View(sol);
        }

        // POST: /Solicitacoes/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var sol = _context.Solicitacoes.Find(id);
            if (sol == null) return NotFound();
            if (!UsuarioPodeAlterar(sol)) return Forbid();

            _context.Solicitacoes.Remove(sol);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Solicitacoes/Responder
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Responder(int solicitacaoId, string conteudo)
        {
            if (!User.Identity!.IsAuthenticated)
                return Challenge();

            var resposta = new Resposta
            {
                SolicitacaoId = solicitacaoId,
                Conteudo = conteudo,
                EmailUsuario = User.Identity.Name!
            };
            _context.Respostas.Add(resposta);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = solicitacaoId });
        }

        // Carrega a lista de categorias para os dropdowns
        private void CarregarCategorias(int? categoriaSelecionada = null)
        {
            var categorias = _context.Categorias.ToList();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nome", categoriaSelecionada);
        }

        // Verifica se o usuário pode editar/excluir a solicitação
        private bool UsuarioPodeAlterar(Solicitacao s) =>
            User.IsInRole("Admin") ||
            s.EmailUsuario == User.Identity?.Name;
    }
}
