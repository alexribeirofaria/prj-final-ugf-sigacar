using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;

namespace App.Controllers
{
    public class AssociadosController : Controller
    {
        private readonly SigacarStore _store;
        public AssociadosController(SigacarStore store) => _store = store;

        public IActionResult Index(string busca, string ordem = "nome")
        {
            var dados = _store.Associados.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(busca))
                dados = dados.Where(a => (a.Nome ?? "").Contains(busca, StringComparison.OrdinalIgnoreCase) || (a.Email ?? "").Contains(busca, StringComparison.OrdinalIgnoreCase) || (a.Cpf ?? "").Contains(busca));

            dados = ordem == "cpf" ? dados.OrderBy(a => a.Cpf) : ordem == "email" ? dados.OrderBy(a => a.Email) : dados.OrderBy(a => a.Nome);
            return View(dados.ToList());
        }

        public IActionResult Details(int id) => View(_store.Associados.FirstOrDefault(a => a.Id == id));
        public IActionResult Create() => View("Form", new Associado());
        public IActionResult Edit(int id) => View("Form", _store.Associados.FirstOrDefault(a => a.Id == id));

        [HttpPost]
        public IActionResult Save(Associado associado)
        {
            if (!ModelState.IsValid)
                return View("Form", associado);
            try
            {
                _store.SalvarAssociado(associado);
                TempData["Mensagem"] = "Associado salvo com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Form", associado);
            }
        }

        public IActionResult Delete(int id)
        {
            _store.RemoverAssociado(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
