using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoFrequenciaMensal : RelatorioPaginadoColuna<FrequenciaMensalDto>
    {
        private const string TODAS = "Todas";
        private const string TODOS = "Todos";

        private readonly FrequenciaMensalCabecalhoDto cabecalho;
        private Dictionary<EnumAgrupamentoFrenquenciaMensal, bool> dicionarioApresentacaoAgrupamento;
        public RelatorioPaginadoFrequenciaMensal(
                            ParametroRelatorioPaginadoPorColuna<FrequenciaMensalDto> parametro,
                            List<IColuna> colunas,
                            FrequenciaMensalCabecalhoDto cabecalho) : base(parametro, colunas)
        {
            this.cabecalho = cabecalho;
            CarregueDicionarioApresentacao();
            this.TotalDeAgrupamentoOculto = this.dicionarioApresentacaoAgrupamento.Where(grupo => grupo.Value).Count();
        }

        private void CarregueDicionarioApresentacao()
        {
            this.dicionarioApresentacaoAgrupamento = new Dictionary<EnumAgrupamentoFrenquenciaMensal, bool>
            {
                { EnumAgrupamentoFrenquenciaMensal.DRE, ParametroEhTodasOuMaisDeUm(this.cabecalho.NomeDre) },
                { EnumAgrupamentoFrenquenciaMensal.UE, ParametroEhTodasOuMaisDeUm(this.cabecalho.NomeUe) },
                { EnumAgrupamentoFrenquenciaMensal.TURMA, ParametroEhTodasOuMaisDeUm(this.cabecalho.NomeTurma) },
                { EnumAgrupamentoFrenquenciaMensal.MES, ParametroEhTodasOuMaisDeUm(this.cabecalho.MesReferencia) }
            };
        }

        private static bool ParametroEhTodasOuMaisDeUm(string valor)
            => (valor?.Equals(TODAS, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
               (valor?.Equals(TODOS, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
               string.IsNullOrWhiteSpace(valor);

        protected override Pagina ObtenhaPagina(int indice, int ordenacao, List<FrequenciaMensalDto> valores, List<IColuna> colunas)
            => new PaginaFrenquenciaMensal()
            {
                Indice = indice,
                Ordenacao = ordenacao,
                Valores = valores,
                Colunas = colunas,
                DicionarioApresentacaoAgrupamento = this.dicionarioApresentacaoAgrupamento
            };
    }
}
