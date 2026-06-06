namespace App.Models
{
    public class Empregado : Pessoa
    {
        public string CodigoIdentificacao { get; set; }
        public string Senha { get; set; }
        public bool Master { get; set; }
    }
}
