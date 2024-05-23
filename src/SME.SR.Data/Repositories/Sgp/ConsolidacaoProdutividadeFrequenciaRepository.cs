using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using Org.BouncyCastle.Ocsp;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConsolidacaoProdutividadeFrequenciaRepository : IConsolidacaoProdutividadeFrequenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
       
        public ConsolidacaoProdutividadeFrequenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }       

        private string ObterCondicaoUe(FiltroRelatorioProdutividadeFrequenciaDto filtro) =>
                    !filtro.CodigoUe.EstaFiltrandoTodas() ? " and ue_id = @CodigoUe " : string.Empty;

        private string ObterCondicaoDre(FiltroRelatorioProdutividadeFrequenciaDto filtro) =>
                    !filtro.CodigoDre.EstaFiltrandoTodas() ? " and dre_id = @CodigoDre " : string.Empty;

        private string ObterCondicaoAnoLetivo(FiltroRelatorioProdutividadeFrequenciaDto filtro) =>
                    filtro.AnoLetivo != 0 ? " and ano_letivo = @AnoLetivo " : string.Empty;

        private string ObterCondicaoBimestre(FiltroRelatorioProdutividadeFrequenciaDto filtro) =>
                    filtro.Bimestre.HasValue ? " and bimestre = @Bimestre " : string.Empty;

        private string ObterCondicaoRfProfessor(FiltroRelatorioProdutividadeFrequenciaDto filtro) =>
                    !string.IsNullOrEmpty(filtro.RfProfessor) ? " and professor_rf = @RfProfessor " : string.Empty;

        private string ObterCondicoes(FiltroRelatorioProdutividadeFrequenciaDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioProdutividadeFrequenciaDto, string>>
            {
                ObterCondicaoUe,
                ObterCondicaoDre,
                ObterCondicaoAnoLetivo,
                ObterCondicaoBimestre,
                ObterCondicaoRfProfessor,
            };

            foreach (var funcao in funcoes)
                query.Append(funcao(filtro));

            return query.ToString();
        }

        public async Task<IEnumerable<ConsolidacaoProdutividadeFrequenciaDto>> ObterConsolidacoesProdutividadeFrequenciaFiltro(FiltroRelatorioProdutividadeFrequenciaDto filtro)
        {
            var query = new StringBuilder();
            query.AppendLine($@"select consol.Id
                                       ,consol.turma_id CodigoTurma 
                                       ,consol.turma_desc DescricaoTurma 
                                       ,consol.ue_id CodigoUe 
                                       ,consol.ue_desc DescricaoUe 
                                       ,consol.dre_id CodigoDre 
                                       ,consol.dre_desc DescricaoDre 
                                       ,consol.professor_nm NomeProfessor 
                                       ,consol.professor_rf RfProfessor 
                                       ,consol.Bimestre 
                                       ,consol.modalidade_codigo Modalidade 
                                       ,consol.data_aula DataAula 
                                       ,consol.data_reg_freq DataRegistroFrequencia 
                                       ,consol.dif_data_aula_reg_freq DiferenciaDiasDataAulaRegistroFrequencia 
                                       ,consol.ano_letivo AnoLetivo 
                                       ,consol.componente_curricular_id CodigoComponenteCurricular 
                                       ,consol.componente_curricular_nm NomeComponenteCurricular 
                                from consolidacao_produtividade_frequencia consol
                                where 1 = 1 ");
            query.AppendLine(ObterCondicoes(filtro));
            query.AppendLine(@"order by consol.dre_id, consol.ue_desc, consol.professor_nm, consol.turma_desc, consol.bimestre ");
            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            
            return await conexao.QueryAsync<ConsolidacaoProdutividadeFrequenciaDto>(query.ToString(),
                new
                {
                    filtro.AnoLetivo,
                    filtro.CodigoDre,
                    filtro.CodigoUe,
                    filtro.Bimestre,
                    filtro.RfProfessor
                });
        }

       
    }
}
