using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoPlanoAee
    {
        private const int TOTAL_LINHAS = 40;
        private readonly CabecalhoPlanosAeeDto Cabecalho;
        private readonly List<AgrupamentoDreUeDto> Agrupamentos;
        private List<RelatorioPlanosAeeDto> RelatorioPaginado;

        private Dictionary<Colunas, int> DicColunaPorQtdeCaracteres;

        public RelatorioPaginadoPlanoAee(CabecalhoPlanosAeeDto cabecalho, List<AgrupamentoDreUeDto> agrupamentos)
        {
            Cabecalho = cabecalho;
            Agrupamentos = agrupamentos;
            DicColunaPorQtdeCaracteres = ObterColunasPorQtdeCaracteres();
        }

        public IEnumerable<RelatorioPlanosAeeDto> ObterRelatorioPaginado()
        {
            return new List<RelatorioPlanosAeeDto>();
        }

        private void AdicionePagina(RelatorioPlanosAeeDto pagina)
        {
            RelatorioPaginado.Add(pagina);
        }

        private void ExecutePaginacao()
        {
            var linhas = 0;
            var novoAgrupamento = new List<AgrupamentoDreUeDto>();
            var pagina = ObterPagina();

            foreach (var grupo in Agrupamentos)
            {
                if (linhas >= TOTAL_LINHAS)
                {
                    AdicionePagina(pagina);
                    linhas = 0;
                    pagina = ObterPagina();
                }

                linhas++;
                pagina.AgrupamentosDreUe.ToList().Add(ObterAgrupamento(grupo));

                foreach(var detalhe in grupo.Detalhes)
                {
                    linhas++;

                }
            }
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

        private (int, List<AgrupamentoDreUeDto>) ObterQtdeLinhaEagrupamento(List<AgrupamentoDreUeDto> agrupamento)
        {
            var linhas = 0;
            var novoAgrupamento = new List<AgrupamentoDreUeDto>();

            foreach(var grupo in agrupamento)
            {

            }

            return (linhas, novoAgrupamento);
        }

        private int ObterLinhasDeQuebra(int linhas, DetalhePlanosAeeDto detalhe)
        {
            Func<DetalhePlanosAeeDto, bool> ColunaCriancaLimiteValido = detalheCrianca => detalheCrianca.Aluno.Length <= DicColunaPorQtdeCaracteres[Colunas.CRIANCA_ESTUDANTE];
            
            
            return linhas;
        }

        private Dictionary<Colunas, int> ObterColunasPorQtdeCaracteres()
        {
            return new Dictionary<Colunas, int>
            {
                {Colunas.CRIANCA_ESTUDANTE, 47},
                {Colunas.SITUACAO, 25},
                {Colunas.RESPONSAVEL, 46},
                {Colunas.PAAI_RESPONSAVEL, 39}
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
