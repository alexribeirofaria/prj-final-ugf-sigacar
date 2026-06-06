namespace App.Models
{
    public class PerfilResultado
    {
        public PerfilTipo Perfil { get; set; }
        public double Pontuacao { get; set; }
        public double Basic { get; set; }
        public double Advanced { get; set; }
        public double Master { get; set; }
        public string Recomendacao { get; set; }
    }
}
