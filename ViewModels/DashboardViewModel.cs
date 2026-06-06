using System.Collections.Generic;
using App.Models;

namespace App.ViewModels
{
    public class DashboardViewModel
    {
        public IReadOnlyList<Veiculo> Veiculos { get; set; }
        public IReadOnlyList<Categoria> Categorias { get; set; }
        public IReadOnlyList<Reserva> Reservas { get; set; }
        public IReadOnlyList<Contrato> Contratos { get; set; }
        public IReadOnlyList<Associado> Associados { get; set; }
    }
}
