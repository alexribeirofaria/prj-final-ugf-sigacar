using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;

namespace App.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly SigacarStore _store;
        public CategoriasController(SigacarStore store) => _store = store;

        public IActionResult Index() => View(_store.Categorias.OrderBy(c => c.Codigo).ToList());
        public IActionResult Create() => View("Form", new Categoria());
        public IActionResult Edit(int id) => View("Form", _store.Categorias.FirstOrDefault(c => c.Id == id));

        [HttpPost]
        public IActionResult Save(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return View("Form", categoria);
            _store.SalvarCategoria(categoria);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            _store.RemoverCategoria(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
