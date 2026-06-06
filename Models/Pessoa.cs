using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public abstract class Pessoa
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(14)]
        public string Cpf { get; set; }

        public string Rg { get; set; }
        public string OrgaoEmissor { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Telefone { get; set; }
        public DateTime DataNascimento { get; set; } = DateTime.Today.AddYears(-30);
    }
}
