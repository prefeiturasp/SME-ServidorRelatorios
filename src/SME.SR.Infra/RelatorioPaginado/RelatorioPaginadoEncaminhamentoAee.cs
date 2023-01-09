using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoEncaminhamentoAee
    {
        private const int TOTAL_LINHAS = 35;
        private readonly CabecalhoEncaminhamentoAeeDto Cabecalho;
        private readonly List<AgrupamentoEncaminhamentoAeeDreUeDto> Agrupamentos;
        private List<RelatorioEncaminhamentoAeeDto> RelatorioPaginado;
        private Dictionary<Colunas, int> DicColunaPorQtdeCaracteres;

        public RelatorioPaginadoEncaminhamentoAee(CabecalhoEncaminhamentoAeeDto cabecalho, List<AgrupamentoEncaminhamentoAeeDreUeDto> agrupamentos)
        {
            Cabecalho = cabecalho;
            Agrupamentos = agrupamentos;
            DicColunaPorQtdeCaracteres = ObterColunasPorQtdeCaracteres();
            RelatorioPaginado = new List<RelatorioEncaminhamentoAeeDto>();
        }

        public IEnumerable<RelatorioEncaminhamentoAeeDto> ObterRelatorioPaginado()
        {
            ExecutarPaginacao();
            return RelatorioPaginado;
        }

        private void AdicionarPagina(RelatorioEncaminhamentoAeeDto pagina)
        {
            RelatorioPaginado.Add(pagina);
        }

        private void ExecutarPaginacao()
        {
            var linhas = 0;
            var pagina = ObterPagina();

            foreach (var grupo in Agrupamentos)
            {
                var agrupamentoAtual = ObterAgrupamento(grupo);
                pagina.AgrupamentosDreUe.Add(agrupamentoAtual);

                foreach (var detalhe in grupo.Detalhes)
                {
                    var linhasPorDetalhe = ObterLinhasDeQuebra(detalhe);

                    linhas += linhasPorDetalhe;

                    if (linhas >= TOTAL_LINHAS)
                    {
                        RemoveAgrupamentoSemDetalhe(pagina);
                        AdicionarPagina(pagina);
                        linhas = linhasPorDetalhe + 1;
                        pagina = ObterPagina();
                        agrupamentoAtual = ObterAgrupamento(grupo);
                        pagina.AgrupamentosDreUe.Add(agrupamentoAtual);
                    }

                    agrupamentoAtual.Detalhes.Add(detalhe);
                }

                linhas += 2;
            }

            if (linhas > 0 && RelatorioPaginado.Count == 0)
                AdicionarPagina(pagina);
        }

        private void RemoveAgrupamentoSemDetalhe(RelatorioEncaminhamentoAeeDto pagina)
        {
            var grupo = pagina.AgrupamentosDreUe.LastOrDefault();

            if (grupo != null && !grupo.Detalhes.Any())
                pagina.AgrupamentosDreUe.Remove(grupo);
        }

        private AgrupamentoEncaminhamentoAeeDreUeDto ObterAgrupamento(AgrupamentoEncaminhamentoAeeDreUeDto grupo)
        {
            return new AgrupamentoEncaminhamentoAeeDreUeDto()
            {
                UeNome = grupo.UeNome,
                DreNome = grupo.DreNome,
                Detalhes = new List<DetalheEncaminhamentoAeeDto>()
            };
        }

        private RelatorioEncaminhamentoAeeDto ObterPagina()
        {
            return new RelatorioEncaminhamentoAeeDto()
            {
                Cabecalho = Cabecalho,
                AgrupamentosDreUe = new List<AgrupamentoEncaminhamentoAeeDreUeDto>()
            };
        }

        private int ObterLinhasDeQuebra(DetalheEncaminhamentoAeeDto detalhe)
        {
            var funcoesLimiteCaracteres = ObterFuncoesLimiteCaracteres();
            return 2 + funcoesLimiteCaracteres.Count(funcao => !funcao(detalhe));
        }

        private IEnumerable<Func<DetalheEncaminhamentoAeeDto, bool>> ObterFuncoesLimiteCaracteres()
        {
            var funcoesLimiteCaracteres = new List<Func<DetalheEncaminhamentoAeeDto, bool>>
            {
                detalhe => detalhe.Situacao.Length <= DicColunaPorQtdeCaracteres[Colunas.SITUACAO] && detalhe.ResponsavelPAAI.Length <= DicColunaPorQtdeCaracteres[Colunas.PAAI_RESPONSAVEL],
                detalhe => detalhe.Aluno.Length <= DicColunaPorQtdeCaracteres[Colunas.CRIANCA_ESTUDANTE]
            };

            return funcoesLimiteCaracteres;
        }

        private Dictionary<Colunas, int> ObterColunasPorQtdeCaracteres()
        {
            return new Dictionary<Colunas, int>
            {
                {Colunas.CRIANCA_ESTUDANTE, 47},
                {Colunas.SITUACAO, 30},
                {Colunas.PAAI_RESPONSAVEL, 41}
            };
        }

        private enum Colunas
        {
            CRIANCA_ESTUDANTE,
            SITUACAO,
            PAAI_RESPONSAVEL
        }
    }
}
