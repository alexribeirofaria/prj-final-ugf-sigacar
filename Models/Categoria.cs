using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required, Display(Name = "Codigo")]
        public string Codigo { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Display(Name = "Diaria")]
        public decimal PrecoDiario { get; set; }

        [Display(Name = "Semanal")]
        public decimal PrecoSemanal { get; set; }

        [Display(Name = "Mensal")]
        public decimal PrecoMensal { get; set; }

        public int QuantidadeEstoque { get; set; }
    }
}
