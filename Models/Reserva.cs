using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int AssociadoId { get; set; }

        [Required]
        public int VeiculoId { get; set; }

        public int CategoriaId { get; set; }
        public string CidadeRetirada { get; set; }
        public string EstadoRetirada { get; set; }
        public string CidadeDevolucao { get; set; }
        public string EstadoDevolucao { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Retirada { get; set; } = DateTime.Today.AddDays(1).AddHours(9);

        [DataType(DataType.DateTime)]
        public DateTime Devolucao { get; set; } = DateTime.Today.AddDays(3).AddHours(18);

        public decimal Desconto { get; set; }
        public bool Cancelada { get; set; }
    }
}
