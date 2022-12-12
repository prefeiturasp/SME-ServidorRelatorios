using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoPlanoAee
    {
        private const int TOTAL_LINHAS = 36;
        private readonly CabecalhoPlanosAeeDto Cabecalho;
        private readonly List<AgrupamentoDreUeDto> Agrupamentos;
        private List<RelatorioPlanosAeeDto> RelatorioPaginado;

        private Dictionary<Colunas, int> DicColunaPorQtdeCaracteres;

        public RelatorioPaginadoPlanoAee(CabecalhoPlanosAeeDto cabecalho, List<AgrupamentoDreUeDto> agrupamentos)
        {
            Cabecalho = cabecalho;
            Agrupamentos = agrupamentos;
            DicColunaPorQtdeCaracteres = ObterColunasPorQtdeCaracteres();
            RelatorioPaginado = new List<RelatorioPlanosAeeDto>();
        }

        public IEnumerable<RelatorioPlanosAeeDto> ObterRelatorioPaginado()
        {
            ExecutePaginacao();

            return RelatorioPaginado;
        }

        private void AdicionePagina(RelatorioPlanosAeeDto pagina)
        {
            RelatorioPaginado.Add(pagina);
        }

        private void ExecutePaginacao()
        {
            var linhas = 0;
            var pagina = ObterPagina();

            foreach (var grupo in Agrupamentos)
            {
                var agrupamentoAtual = ObterAgrupamento(grupo);
                pagina.AgrupamentosDreUe.Add(agrupamentoAtual);

                foreach(var detalhe in grupo.Detalhes)
                {
                    var linhasPorDetalhe = ObterLinhasDeQuebra(detalhe);

                    linhas += linhasPorDetalhe;

                    if (linhas >= TOTAL_LINHAS)
                    {
                        RemoveAgrupamentoSemDetalhe(pagina);
                        AdicionePagina(pagina);
                        linhas = linhasPorDetalhe + 1;
                        pagina = ObterPagina();
                        agrupamentoAtual = ObterAgrupamento(grupo);
                        pagina.AgrupamentosDreUe.Add(agrupamentoAtual);
                    }

                    agrupamentoAtual.Detalhes.Add(detalhe);
                }

                linhas += 2;
            }
        }

        private void RemoveAgrupamentoSemDetalhe(RelatorioPlanosAeeDto pagina)
        {
            var grupo = pagina.AgrupamentosDreUe.LastOrDefault();

            if (!grupo.Detalhes.Any())
                pagina.AgrupamentosDreUe.Remove(grupo);
        }

        private AgrupamentoDreUeDto ObterAgrupamento(AgrupamentoDreUeDto grupo)
        {
            return new AgrupamentoDreUeDto()
            {
                UeNome = grupo.UeNome,
                DreNome= grupo.DreNome,
                Detalhes = new List<DetalhePlanosAeeDto>()    
            };
        }

        private RelatorioPlanosAeeDto ObterPagina()
        {
            return new RelatorioPlanosAeeDto()
            {
                Cabecalho = Cabecalho,
                AgrupamentosDreUe = new List<AgrupamentoDreUeDto>()
            };
        }

        private int ObterLinhasDeQuebra(DetalhePlanosAeeDto detalhe)
        {
            var funcoesLimiteCaracteres = ObterFuncoesLimiteCaracteres();
            var linhas = 3;

            foreach (var funcao in funcoesLimiteCaracteres)
            {
                if (!funcao(detalhe))
                    linhas++;
            }
           
            return linhas;
        }

        private List<Func<DetalhePlanosAeeDto, bool>> ObterFuncoesLimiteCaracteres()
        {
            var funcoesLimiteCaracteres = new List<Func<DetalhePlanosAeeDto, bool>>();

            funcoesLimiteCaracteres.Add(detalhe => detalhe.Situacao.Length <= DicColunaPorQtdeCaracteres[Colunas.SITUACAO] && detalhe.Responsavel.Length <= DicColunaPorQtdeCaracteres[Colunas.RESPONSAVEL]);
            funcoesLimiteCaracteres.Add(detalhe => detalhe.Aluno.Length <= DicColunaPorQtdeCaracteres[Colunas.CRIANCA_ESTUDANTE]);
            funcoesLimiteCaracteres.Add(detalhe => detalhe.ResponsavelPAAI.Length <= DicColunaPorQtdeCaracteres[Colunas.PAAI_RESPONSAVEL]);

            return funcoesLimiteCaracteres;
        }

        private Dictionary<Colunas, int> ObterColunasPorQtdeCaracteres()
        {
            return new Dictionary<Colunas, int>
            {
                {Colunas.CRIANCA_ESTUDANTE, 47},
                {Colunas.SITUACAO, 30},
                {Colunas.RESPONSAVEL, 46},
                {Colunas.PAAI_RESPONSAVEL, 42}
            };
        }

        private enum Colunas
        {
            CRIANCA_ESTUDANTE,
            SITUACAO,
            RESPONSAVEL,
            PAAI_RESPONSAVEL
        }
    }
}
