using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Helpdesk.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Últimas 5 solicitações
            var ultimasSolicitacoes = _context.Solicitacoes
                .OrderByDescending(s => s.DataAbertura)
                .Take(5)
                .ToList();

            // Ranking de usuários baseado em respostas
            var ranking = _context.Respostas
                .GroupBy(r => r.EmailUsuario)
                .Select(g => new
                {
                    Email = g.Key,
                    Pontos = g.Sum(r => r.Melhor ? 2 : 1)
                })
                .OrderByDescending(x => x.Pontos)
                .Take(5)
                .ToList();

            ViewBag.Ranking = ranking;
            return View(ultimasSolicitacoes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
