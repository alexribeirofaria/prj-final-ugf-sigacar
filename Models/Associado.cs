using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Associado : Pessoa
    {
        [Required]
        public string Cnh { get; set; }

        [DataType(DataType.Date)]
        public DateTime ValidadeCnh { get; set; } = DateTime.Today.AddYears(3);

        public bool PossuiPendencia { get; set; }
        public decimal Salario { get; set; }
        public int AnosCarteira { get; set; }
        public PerfilTipo? Perfil { get; set; }
        public double PontuacaoPerfil { get; set; }
    }
}
