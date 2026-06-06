using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        [Required, StringLength(8)]
        public string Placa { get; set; }

        [Required]
        public string Fabricante { get; set; }

        public string Marca { get; set; }
        public int Ano { get; set; }

        [Required]
        public string Modelo { get; set; }

        public int CategoriaId { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
