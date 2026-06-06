using System.Collections.Generic;
using App.Models;

namespace App.ViewModels
{
    public class ContratoFormViewModel
    {
        public int ReservaId { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public string NumeroCartao { get; set; }
        public string CodigoSeguranca { get; set; }
        public string Observacao { get; set; }
        public IReadOnlyList<Reserva> Reservas { get; set; }
    }
}
