using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;

namespace App.Controllers
{
    public class EmpregadosController : Controller
    {
        private readonly SigacarStore _store;
        public EmpregadosController(SigacarStore store) => _store = store;

        public IActionResult Index() => View(_store.Empregados.OrderBy(e => e.Nome).ToList());
        public IActionResult Create() => View("Form", new Empregado());
        public IActionResult Edit(int id) => View("Form", _store.Empregados.FirstOrDefault(e => e.Id == id));

        [HttpPost]
        public IActionResult Save(Empregado empregado)
        {
            if (!ModelState.IsValid)
                return View("Form", empregado);
            try
            {
                _store.SalvarEmpregado(empregado);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Form", empregado);
            }
        }

        public IActionResult Delete(int id)
        {
            _store.RemoverEmpregado(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
