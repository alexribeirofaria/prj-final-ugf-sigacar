using System;

namespace App.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public DateTime DataAssinatura { get; set; } = DateTime.Now;
        public ContratoStatus Status { get; set; } = ContratoStatus.Contratado;
        public string Observacao { get; set; }
        public decimal Total { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public string Comprovante { get; set; }
    }
}
