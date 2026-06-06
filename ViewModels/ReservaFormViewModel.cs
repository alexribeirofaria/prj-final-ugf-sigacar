using System.Collections.Generic;
using App.Models;

namespace App.ViewModels
{
    public class ReservaFormViewModel
    {
        public Reserva Reserva { get; set; } = new Reserva();
        public IReadOnlyList<Associado> Associados { get; set; }
        public IReadOnlyList<Veiculo> Veiculos { get; set; }
    }
}
