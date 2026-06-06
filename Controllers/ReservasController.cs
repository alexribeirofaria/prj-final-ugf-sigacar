using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;
using App.ViewModels;

namespace App.Controllers
{
    public class ReservasController : Controller
    {
        private readonly SigacarStore _store;
        public ReservasController(SigacarStore store) => _store = store;

        public IActionResult Index(string busca)
        {
            ViewBag.Associados = _store.Associados;
            ViewBag.Veiculos = _store.Veiculos;
            var dados = _store.Reservas.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(busca))
                dados = dados.Where(r =>
                    (_store.Associados.FirstOrDefault(a => a.Id == r.AssociadoId)?.Cpf ?? "").Contains(busca) ||
                    (_store.Veiculos.FirstOrDefault(v => v.Id == r.VeiculoId)?.Placa ?? "").Contains(busca, StringComparison.OrdinalIgnoreCase));
            return View(dados.OrderByDescending(r => r.Retirada).ToList());
        }

        public IActionResult Create() => View("Form", MontarForm(new Reserva()));
        public IActionResult Edit(int id) => View("Form", MontarForm(_store.Reservas.FirstOrDefault(r => r.Id == id)));

        [HttpPost]
        public IActionResult Save(Reserva reserva)
        {
            try
            {
                _store.SalvarReserva(reserva);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Form", MontarForm(reserva));
            }
        }

        public IActionResult Delete(int id)
        {
            _store.CancelarReserva(id);
            return RedirectToAction(nameof(Index));
        }

        private ReservaFormViewModel MontarForm(Reserva reserva) => new ReservaFormViewModel
        {
            Reserva = reserva ?? new Reserva(),
            Associados = _store.Associados,
            Veiculos = _store.Veiculos.Where(v => v.Ativo).ToList()
        };
    }
}
