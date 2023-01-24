using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class EncaminhamentoAeeRepository : IEncaminhamentoAeeRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public EncaminhamentoAeeRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<EncaminhamentoAeeDto>> ObterEncaminhamentosAEE(FiltroRelatorioEncaminhamentosAeeDto filtro)
        {
            var query = new StringBuilder();

            query.AppendLine(@"select ea.id, d.dre_id dreId, 
			        d.abreviacao as dreAbreviacao,
			        u.ue_id as ueCodigo,
			        u.nome as ueNome,
			        u.tipo_escola as tipoEscola,
			        ea.aluno_codigo as alunoCodigo,
			        ea.aluno_nome as alunoNome,
			        t.turma_id as turmaCodigo,
			        t.nome as turmaNome,
			        t.ano_letivo as anoLetivo,
			        t.modalidade_codigo as modalidade,
			        ea.situacao as situacao,
			        responsavel.nome as ResponsavelPaaiNome,
			        coalesce(responsavel.login, responsavel.rf_codigo) as ResponsavelPaaiLoginRf
		        from encaminhamento_aee ea
		        inner join turma t on t.id = ea.turma_id
					inner join ue u on u.id = t.ue_id 
					inner join dre d on d.id = u.dre_id
					left join usuario responsavel on responsavel.id = ea.responsavel_id
                where not ea.excluido ");

            query.AppendLine(ObterCondicao(filtro));

            query.AppendLine(" order by d.dre_id ");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<EncaminhamentoAeeDto>(query.ToString(), new
            {
                dreCodigo = filtro.DreCodigo,
                modalidade = filtro.Modalidade,
                ueCodigo = filtro.UeCodigo,
                situacaoIds = filtro.SituacaoIds,
                codigosPAAIResponsavel = filtro.CodigosPAAIResponsavel,
                semestre = filtro.Semestre,
                codigosTurma = filtro.CodigosTurma
            });
        }

        private string ObterCondicaoDre(FiltroRelatorioEncaminhamentosAeeDto filtro) =>
                    !filtro.DreCodigo.EstaFiltrandoTodas() ? " and d.dre_id = @dreCodigo " : string.Empty;

        private string ObterCondicaoUe(FiltroRelatorioEncaminhamentosAeeDto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;

        private string ObterCondicaoModalidade(FiltroRelatorioEncaminhamentosAeeDto filtro) =>
                    !filtro.Modalidade.EstaFiltrandoTodas() ? " and t.modalidade_codigo = @modalidade " : string.Empty;

        private string ObterCondicaoSemestre(FiltroRelatorioEncaminhamentosAeeDto filtro) =>
                    filtro.Semestre > 0 ? " and t.semestre = @semestre " : string.Empty;

        private string ObterCodicaoTurma(FiltroRelatorioEncaminhamentosAeeDto filtro) =>
                    !filtro.CodigosTurma.EstaFiltrandoTodas() ? " and t.turma_id = ANY(@codigosTurma) " : string.Empty;

        private string ObterCodicaoSituacao(FiltroRelatorioEncaminhamentosAeeDto filtro)  
        {
            var condicao = string.Empty;

            if (!filtro.ExibirEncerrados)
                condicao += $" and ea.situacao <> {(int)SituacaoEncaminhamentoAEE.EncerradoAutomaticamente}";
            if (!filtro.SituacaoIds.EstaFiltrandoTodas())
                condicao += " and ea.situacao = ANY(@situacaoIds) ";

            return condicao;
        }

        private string ObterCodicaoPAAI(FiltroRelatorioEncaminhamentosAeeDto filtro) =>
        	       !filtro.CodigosPAAIResponsavel.EstaFiltrandoTodas() ? " and coalesce(responsavel.login, responsavel.rf_codigo) = ANY(@codigosPAAIResponsavel) " : string.Empty;

        private string ObterCondicao(FiltroRelatorioEncaminhamentosAeeDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioEncaminhamentosAeeDto, string>>
            {
                ObterCondicaoDre,
                ObterCondicaoUe,
                ObterCondicaoModalidade,
                ObterCondicaoSemestre,
                ObterCodicaoTurma,
                ObterCodicaoSituacao,
                ObterCodicaoPAAI
            };

            foreach(var funcao in funcoes)
            {
                query.Append(funcao(filtro));
            }

            return query.ToString();
        }

        public async Task<IEnumerable<EncaminhamentoAeeDto>> ObterEncaminhamentosAEEPorIds(long[] encaminhamentosAeeId)
        {
            var query = new StringBuilder();

            query.AppendLine(@"select ea.id, d.dre_id dreId, 
			        d.abreviacao as dreAbreviacao,
			        u.ue_id as ueCodigo,
			        u.nome as ueNome,
			        u.tipo_escola as tipoEscola,
			        ea.aluno_codigo as alunoCodigo,
			        ea.aluno_nome as alunoNome,
			        t.turma_id as turmaCodigo,
			        t.nome as turmaNome,
			        t.ano_letivo as anoLetivo,
			        t.modalidade_codigo as modalidade,
			        ea.situacao as situacao,
			        responsavel.nome as ResponsavelPaaiNome,
			        coalesce(responsavel.login, responsavel.rf_codigo) as ResponsavelPaaiLoginRf,
                    ea.criado_em as CriadoEm
		        from encaminhamento_aee ea
		        inner join turma t on t.id = ea.turma_id
					inner join ue u on u.id = t.ue_id 
					inner join dre d on d.id = u.dre_id
					left join usuario responsavel on responsavel.id = ea.responsavel_id
                where ea.id = any(@encaminhamentosAeeId)");

            query.AppendLine(" order by d.dre_id ");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<EncaminhamentoAeeDto>(query.ToString(), new
            {
                encaminhamentosAeeId
            });
        }
    }
}
