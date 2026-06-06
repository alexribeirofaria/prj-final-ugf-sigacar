using Microsoft.AspNetCore.Mvc;
using App.Services;
using App.ViewModels;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        private readonly SigacarStore _store;

        public HomeController(SigacarStore store) => _store = store;

        public IActionResult Index()
        {
            return View(new DashboardViewModel
            {
                Veiculos = _store.Veiculos,
                Categorias = _store.Categorias,
                Reservas = _store.Reservas,
                Contratos = _store.Contratos,
                Associados = _store.Associados
            });
        }
    }
}
