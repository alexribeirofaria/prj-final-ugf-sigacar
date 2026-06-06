using System;
using System.Collections.Generic;
using System.Linq;
using App.Models;

namespace App.Services
{
    public class SigacarStore
    {
        private readonly List<Associado> _associados = new List<Associado>();
        private readonly List<Empregado> _empregados = new List<Empregado>();
        private readonly List<Categoria> _categorias = new List<Categoria>();
        private readonly List<Veiculo> _veiculos = new List<Veiculo>();
        private readonly List<Reserva> _reservas = new List<Reserva>();
        private readonly List<Contrato> _contratos = new List<Contrato>();

        public SigacarStore()
        {
            Seed();
        }

        public IReadOnlyList<Associado> Associados => _associados;
        public IReadOnlyList<Empregado> Empregados => _empregados;
        public IReadOnlyList<Categoria> Categorias => _categorias;
        public IReadOnlyList<Veiculo> Veiculos => _veiculos;
        public IReadOnlyList<Reserva> Reservas => _reservas.Where(r => !r.Cancelada).ToList();
        public IReadOnlyList<Contrato> Contratos => _contratos;

        public Categoria CategoriaDoVeiculo(int veiculoId)
        {
            var veiculo = _veiculos.FirstOrDefault(v => v.Id == veiculoId);
            return veiculo == null ? null : _categorias.FirstOrDefault(c => c.Id == veiculo.CategoriaId);
        }

        public decimal CalcularTotal(Reserva reserva)
        {
            var categoria = CategoriaDoVeiculo(reserva.VeiculoId);
            if (categoria == null)
                return 0;

            var dias = Math.Max(1, (int)Math.Ceiling((reserva.Devolucao - reserva.Retirada).TotalDays));
            var total = dias * categoria.PrecoDiario;
            return Math.Max(0, total - reserva.Desconto);
        }

        public bool VeiculoDisponivel(int veiculoId, DateTime retirada, DateTime devolucao, int? reservaIgnorada = null)
        {
            return !_reservas.Any(r =>
                !r.Cancelada &&
                r.VeiculoId == veiculoId &&
                r.Id != reservaIgnorada &&
                retirada < r.Devolucao &&
                devolucao > r.Retirada);
        }

        public void SalvarAssociado(Associado associado)
        {
            if (_associados.Any(a => a.Cpf == associado.Cpf && a.Id != associado.Id))
                throw new InvalidOperationException("Ja existe associado cadastrado com este CPF.");
            if (_associados.Any(a => a.Email == associado.Email && a.Id != associado.Id))
                throw new InvalidOperationException("Ja existe associado cadastrado com este e-mail.");

            Upsert(_associados, associado);
        }

        public void SalvarEmpregado(Empregado empregado)
        {
            if (_empregados.Any(e => e.Cpf == empregado.Cpf && e.Id != empregado.Id))
                throw new InvalidOperationException("Ja existe empregado cadastrado com este CPF.");

            Upsert(_empregados, empregado);
        }

        public void SalvarCategoria(Categoria categoria) => Upsert(_categorias, categoria);

        public void SalvarVeiculo(Veiculo veiculo)
        {
            if (_veiculos.Any(v => v.Placa == veiculo.Placa && v.Id != veiculo.Id))
                throw new InvalidOperationException("Ja existe veiculo cadastrado com esta placa.");

            Upsert(_veiculos, veiculo);
        }

        public void SalvarReserva(Reserva reserva)
        {
            var associado = _associados.FirstOrDefault(a => a.Id == reserva.AssociadoId);
            var veiculo = _veiculos.FirstOrDefault(v => v.Id == reserva.VeiculoId && v.Ativo);

            if (associado == null)
                throw new InvalidOperationException("Associado nao localizado.");
            if (associado.PossuiPendencia)
                throw new InvalidOperationException("Associado possui pendencias e deve procurar a central.");
            if (veiculo == null)
                throw new InvalidOperationException("Veiculo nao localizado ou inativo.");
            if (reserva.Devolucao <= reserva.Retirada)
                throw new InvalidOperationException("Data de devolucao deve ser posterior a retirada.");
            if (!VeiculoDisponivel(reserva.VeiculoId, reserva.Retirada, reserva.Devolucao, reserva.Id == 0 ? (int?)null : reserva.Id))
                throw new InvalidOperationException("Veiculo indisponivel no periodo solicitado.");

            reserva.CategoriaId = veiculo.CategoriaId;
            Upsert(_reservas, reserva);
        }

        public Contrato FecharContrato(int reservaId, FormaPagamento formaPagamento, string numeroCartao, string codigoSeguranca, string observacao)
        {
            var reserva = _reservas.FirstOrDefault(r => r.Id == reservaId && !r.Cancelada);
            if (reserva == null)
                throw new InvalidOperationException("Reserva nao localizada.");
            if (_contratos.Any(c => c.ReservaId == reservaId))
                throw new InvalidOperationException("Ja existe contrato para esta reserva.");
            if (formaPagamento == FormaPagamento.Cartao && !CartaoValido(numeroCartao, codigoSeguranca))
                throw new InvalidOperationException("Cartao invalido.");

            var contrato = new Contrato
            {
                Id = ProximoId(_contratos),
                ReservaId = reservaId,
                FormaPagamento = formaPagamento,
                Observacao = observacao,
                Total = CalcularTotal(reserva),
                Comprovante = formaPagamento == FormaPagamento.Boleto
                    ? "Boleto gerado para pagamento."
                    : "Pagamento por cartao aprovado."
            };
            _contratos.Add(contrato);
            return contrato;
        }

        public void RemoverAssociado(int id) => _associados.RemoveAll(a => a.Id == id);
        public void RemoverEmpregado(int id) => _empregados.RemoveAll(e => e.Id == id);
        public void RemoverCategoria(int id) => _categorias.RemoveAll(c => c.Id == id);
        public void RemoverVeiculo(int id) => _veiculos.RemoveAll(v => v.Id == id);

        public void CancelarReserva(int id)
        {
            var reserva = _reservas.FirstOrDefault(r => r.Id == id);
            if (reserva != null)
                reserva.Cancelada = true;
        }

        private static bool CartaoValido(string numero, string codigo)
        {
            var digitos = new string((numero ?? string.Empty).Where(char.IsDigit).ToArray());
            return digitos.Length >= 13 && digitos.Length <= 19 && !string.IsNullOrWhiteSpace(codigo) && codigo.Length >= 3;
        }

        private static void Upsert<T>(List<T> lista, T item) where T : class
        {
            dynamic atual = item;
            if (atual.Id == 0)
            {
                atual.Id = ProximoId(lista);
                lista.Add(item);
                return;
            }

            var index = lista.FindIndex(x => ((dynamic)x).Id == atual.Id);
            if (index >= 0)
                lista[index] = item;
            else
                lista.Add(item);
        }

        private static int ProximoId<T>(IEnumerable<T> lista) where T : class =>
            lista.Any() ? lista.Max(x => (int)((dynamic)x).Id) + 1 : 1;

        private void Seed()
        {
            _categorias.AddRange(new[]
            {
                new Categoria { Id = 1, Codigo = "ECO", Descricao = "Economico", PrecoDiario = 89, PrecoSemanal = 560, PrecoMensal = 2100, QuantidadeEstoque = 2 },
                new Categoria { Id = 2, Codigo = "SED", Descricao = "Sedan", PrecoDiario = 149, PrecoSemanal = 920, PrecoMensal = 3400, QuantidadeEstoque = 2 },
                new Categoria { Id = 3, Codigo = "SUV", Descricao = "SUV Executivo", PrecoDiario = 239, PrecoSemanal = 1480, PrecoMensal = 5600, QuantidadeEstoque = 1 }
            });

            _veiculos.AddRange(new[]
            {
                new Veiculo { Id = 1, Placa = "SIG1A31", Fabricante = "Fiat", Marca = "Fiat", Modelo = "Argo", Ano = 2022, CategoriaId = 1 },
                new Veiculo { Id = 2, Placa = "GAM2B31", Fabricante = "Hyundai", Marca = "Hyundai", Modelo = "HB20", Ano = 2023, CategoriaId = 1 },
                new Veiculo { Id = 3, Placa = "UGF3C31", Fabricante = "Toyota", Marca = "Toyota", Modelo = "Corolla", Ano = 2021, CategoriaId = 2 },
                new Veiculo { Id = 4, Placa = "CAR4D31", Fabricante = "Jeep", Marca = "Jeep", Modelo = "Compass", Ano = 2024, CategoriaId = 3 }
            });

            _associados.Add(new Associado
            {
                Id = 1,
                Nome = "Pedro de Assis Motta",
                Email = "pedro@sigacar.local",
                Cpf = "11122233344",
                Cnh = "CNH123456",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                Telefone = "(21) 99999-0000",
                Salario = 1440,
                AnosCarteira = 10
            });

            _empregados.Add(new Empregado
            {
                Id = 1,
                Nome = "Administrador SIGACAR",
                Email = "admin@sigacar.local",
                Cpf = "00011122233",
                CodigoIdentificacao = "ADM001",
                Senha = "admin",
                Master = true
            });
        }
    }
}
