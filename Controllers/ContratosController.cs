using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Services;
using App.ViewModels;

namespace App.Controllers
{
    public class ContratosController : Controller
    {
        private readonly SigacarStore _store;
        public ContratosController(SigacarStore store) => _store = store;

        public IActionResult Index(string situacao)
        {
            ViewBag.Reservas = _store.Reservas;
            ViewBag.Associados = _store.Associados;
            ViewBag.Veiculos = _store.Veiculos;
            var dados = _store.Contratos.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(situacao))
                dados = dados.Where(c => c.Status.ToString().Contains(situacao, StringComparison.OrdinalIgnoreCase));
            return View(dados.OrderByDescending(c => c.DataAssinatura).ToList());
        }

        public IActionResult Create(int? reservaId) => View("Form", MontarForm(new ContratoFormViewModel { ReservaId = reservaId ?? 0 }));

        [HttpPost]
        public IActionResult Save(ContratoFormViewModel form)
        {
            try
            {
                _store.FecharContrato(form.ReservaId, form.FormaPagamento, form.NumeroCartao, form.CodigoSeguranca, form.Observacao);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Form", MontarForm(form));
            }
        }

        private ContratoFormViewModel MontarForm(ContratoFormViewModel form)
        {
            form.Reservas = _store.Reservas.Where(r => !_store.Contratos.Any(c => c.ReservaId == r.Id)).ToList();
            return form;
        }
    }
}
