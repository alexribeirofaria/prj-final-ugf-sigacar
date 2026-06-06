using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class PerfilQuestionario
    {
        [Required]
        public int AssociadoId { get; set; }

        [Range(16, 90)]
        public int Idade { get; set; } = 28;

        [Range(0, 100000)]
        public decimal Salario { get; set; } = 1440;

        [Range(0, 70), Display(Name = "Anos de CNH")]
        public int AnosCarteira { get; set; } = 10;
    }
}
