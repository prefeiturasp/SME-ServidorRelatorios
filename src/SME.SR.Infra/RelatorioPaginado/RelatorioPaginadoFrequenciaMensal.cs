using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoFrequenciaMensal : RelatorioPaginadoColuna<FrequenciaMensalDto>
    {
        private const string TODAS = "Todas";
        private const string TODOS = "Todos";

        private FrequenciaMensalCabecalhoDto cabecalho;
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
            this.dicionarioApresentacaoAgrupamento = new Dictionary<EnumAgrupamentoFrenquenciaMensal, bool>();
            this.dicionarioApresentacaoAgrupamento.Add(EnumAgrupamentoFrenquenciaMensal.DRE, ParametroEhTodas(this.cabecalho.NomeDre));
            this.dicionarioApresentacaoAgrupamento.Add(EnumAgrupamentoFrenquenciaMensal.UE, ParametroEhTodas(this.cabecalho.NomeUe));
            this.dicionarioApresentacaoAgrupamento.Add(EnumAgrupamentoFrenquenciaMensal.TURMA, ParametroEhTodas(this.cabecalho.NomeTurma));
            this.dicionarioApresentacaoAgrupamento.Add(EnumAgrupamentoFrenquenciaMensal.MES, ParametroEhTodas(this.cabecalho.MesReferencia));
        }

        private bool ParametroEhTodas(string valor)
        {
            return valor == TODAS || valor == TODOS;
        }

        protected override Pagina ObtenhaPagina(int indice, int ordenacao, List<FrequenciaMensalDto> valores, List<IColuna> colunas)
        {
            return new PaginaFrenquenciaMensal()
            {
                Indice = indice,
                Ordenacao = ordenacao,
                Valores = valores,
                Colunas = colunas,
                DicionarioApresentacaoAgrupamento = this.dicionarioApresentacaoAgrupamento
            };
        }
    }
}
