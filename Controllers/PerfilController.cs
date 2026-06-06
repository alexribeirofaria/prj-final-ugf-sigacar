using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Services;

namespace App.Controllers
{
    public class PerfilController : Controller
    {
        private readonly SigacarStore _store;
        private readonly PerfilFuzzyService _fuzzy;

        public PerfilController(SigacarStore store, PerfilFuzzyService fuzzy)
        {
            _store = store;
            _fuzzy = fuzzy;
        }

        public IActionResult Index()
        {
            ViewBag.Associados = _store.Associados;
            return View(new PerfilQuestionario { AssociadoId = _store.Associados.FirstOrDefault()?.Id ?? 0 });
        }

        [HttpPost]
        public IActionResult Index(PerfilQuestionario questionario)
        {
            ViewBag.Associados = _store.Associados;
            if (!ModelState.IsValid)
                return View(questionario);

            var resultado = _fuzzy.GerarPerfil(questionario);
            var associado = _store.Associados.FirstOrDefault(a => a.Id == questionario.AssociadoId);
            if (associado == null)
            {
                ModelState.AddModelError(string.Empty, "Associado nao localizado.");
                return View(questionario);
            }

            associado.Salario = questionario.Salario;
            associado.AnosCarteira = questionario.AnosCarteira;
            associado.DataNascimento = DateTime.Today.AddYears(-questionario.Idade);
            associado.Perfil = resultado.Perfil;
            associado.PontuacaoPerfil = resultado.Pontuacao;
            ViewBag.Resultado = resultado;
            return View(questionario);
        }
    }
}
