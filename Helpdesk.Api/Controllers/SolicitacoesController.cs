using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;

namespace Helpdesk.Api.Controllers
{
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
    }
}
