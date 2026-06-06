using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;

namespace App.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly SigacarStore _store;
        public VeiculosController(SigacarStore store) => _store = store;

        public IActionResult Index(string ordem = "categoria")
        {
            ViewBag.Categorias = _store.Categorias;
            var dados = _store.Veiculos.AsEnumerable();
            dados = ordem == "preco"
                ? dados.OrderBy(v => _store.CategoriaDoVeiculo(v.Id)?.PrecoDiario ?? 0)
                : ordem == "placa" ? dados.OrderBy(v => v.Placa) : dados.OrderBy(v => _store.CategoriaDoVeiculo(v.Id)?.Descricao);
            return View(dados.ToList());
        }

        public IActionResult Details(int id)
        {
            ViewBag.Categorias = _store.Categorias;
            return View(_store.Veiculos.FirstOrDefault(v => v.Id == id));
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = _store.Categorias;
            return View("Form", new Veiculo { Ano = DateTime.Today.Year });
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Categorias = _store.Categorias;
            return View("Form", _store.Veiculos.FirstOrDefault(v => v.Id == id));
        }

        [HttpPost]
        public IActionResult Save(Veiculo veiculo)
        {
            ViewBag.Categorias = _store.Categorias;
            if (!ModelState.IsValid)
                return View("Form", veiculo);
            try
            {
                _store.SalvarVeiculo(veiculo);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Form", veiculo);
            }
        }

        public IActionResult Delete(int id)
        {
            _store.RemoverVeiculo(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
