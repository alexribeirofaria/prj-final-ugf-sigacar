using System;
using App.Models;

namespace App.Services
{
    public class PerfilFuzzyService
    {
        public PerfilResultado GerarPerfil(PerfilQuestionario questionario)
        {
            var muitoJovem = Triangular(questionario.Idade, 16, 20, 39);
            var jovem = Triangular(questionario.Idade, 20, 30, 45);
            var adulto = Triangular(questionario.Idade, 30, 50, 70);
            var idoso = Triangular(questionario.Idade, 55, 75, 90);

            var salarioBaixo = Decrescente((double)questionario.Salario, 380, 1400);
            var salarioMedio = Triangular((double)questionario.Salario, 760, 1800, 2600);
            var salarioAlto = Crescente((double)questionario.Salario, 1900, 3800);

            var baixoRisco = Crescente(questionario.AnosCarteira, 10, 25);
            var medioRisco = Triangular(questionario.AnosCarteira, 1, 10, 20);
            var altoRisco = Decrescente(questionario.AnosCarteira, 1, 12);

            var basic = Math.Max(
                Math.Min(Math.Min(muitoJovem, salarioBaixo), altoRisco),
                Math.Min(Math.Min(jovem, salarioMedio), altoRisco));

            var advanced = Math.Max(
                Math.Max(Math.Min(Math.Min(muitoJovem, salarioMedio), baixoRisco), Math.Min(Math.Min(jovem, salarioAlto), baixoRisco)),
                Math.Max(Math.Min(Math.Min(adulto, salarioMedio), altoRisco), Math.Min(Math.Min(idoso, salarioMedio), altoRisco)));

            advanced = Math.Max(advanced, Math.Max(Math.Min(Math.Min(adulto, salarioAlto), baixoRisco), Math.Min(Math.Min(idoso, salarioAlto), baixoRisco)));

            var master = Math.Max(
                Math.Min(Math.Min(muitoJovem, salarioAlto), medioRisco),
                Math.Min(Math.Min(jovem, salarioAlto), medioRisco));

            var denominador = basic + advanced + master;
            var pontos = denominador <= 0
                ? 0
                : ((basic * 350) + (advanced * 900) + (master * 1800)) / denominador;

            var perfil = pontos < 700 ? PerfilTipo.Basic : pontos < 1300 ? PerfilTipo.Advanced : PerfilTipo.Master;

            return new PerfilResultado
            {
                Perfil = perfil,
                Pontuacao = Math.Round(pontos, 2),
                Basic = Math.Round(basic, 2),
                Advanced = Math.Round(advanced, 2),
                Master = Math.Round(master, 2),
                Recomendacao = ObterRecomendacao(perfil)
            };
        }

        private static string ObterRecomendacao(PerfilTipo perfil)
        {
            switch (perfil)
            {
                case PerfilTipo.Master:
                    return "SUVs, executivos, mensalidade promocional e prioridade de atendimento.";
                case PerfilTipo.Advanced:
                    return "Sedans, compactos premium, pacotes semanais e descontos progressivos.";
                default:
                    return "Economicos, diarias promocionais e tarifas de entrada.";
            }
        }

        private static double Triangular(double valor, double inicio, double pico, double fim)
        {
            if (valor <= inicio || valor >= fim)
                return 0;
            if (Math.Abs(valor - pico) < 0.0001)
                return 1;
            return valor < pico
                ? (valor - inicio) / (pico - inicio)
                : (fim - valor) / (fim - pico);
        }

        private static double Crescente(double valor, double inicio, double fim)
        {
            if (valor <= inicio)
                return 0;
            if (valor >= fim)
                return 1;
            return (valor - inicio) / (fim - inicio);
        }

        private static double Decrescente(double valor, double inicio, double fim)
        {
            if (valor <= inicio)
                return 1;
            if (valor >= fim)
                return 0;
            return (fim - valor) / (fim - inicio);
        }
    }
}
