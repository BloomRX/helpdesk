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

        // Exibe as últimas 5 solicitações na home
        public IActionResult Index()
        {
            var ultimasSolicitacoes = _context.Solicitacoes
                .OrderByDescending(s => s.DataAbertura)
                .Take(5)
                .ToList();

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
