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

        public async Task<IEnumerable<EncaminhamentoAeeDto>> ObterEncaminhamentosAEE(FiltroRelatorioEncaminhamentoAeeDto filtro)
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
                where 1 = 1	");

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

        private string ObterCondicaoDre(FiltroRelatorioEncaminhamentoAeeDto filtro) =>
                    !filtro.DreCodigo.EstaFiltrandoTodas() ? " and d.dre_id = @dreCodigo " : string.Empty;

        private string ObterCondicaoUe(FiltroRelatorioEncaminhamentoAeeDto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;

        private string ObterCondicaoModalidade(FiltroRelatorioEncaminhamentoAeeDto filtro) =>
                    !filtro.Modalidade.EstaFiltrandoTodas() ? " and t.modalidade_codigo = @modalidade " : string.Empty;

        private string ObterCondicaoSemestre(FiltroRelatorioEncaminhamentoAeeDto filtro) =>
                    filtro.Semestre > 0 ? " and t.semestre = @semestre " : string.Empty;

        private string ObterCodicaoTurma(FiltroRelatorioEncaminhamentoAeeDto filtro) =>
                    !filtro.CodigosTurma.EstaFiltrandoTodas() ? " and t.turma_id = ANY(@codigosTurma) " : string.Empty;

        private string ObterCodicaoSituacao(FiltroRelatorioEncaminhamentoAeeDto filtro)  
        {
            if (filtro.ExibirEncerrados)
                return $" and ea.situacao = {(int)SituacaoEncaminhamentoAEE.EncerradoAutomaticamente}";
            else if (!filtro.SituacaoIds.EstaFiltrandoTodas())
                return " and ea.situacao = ANY(@situacaoIds) ";

            return string.Empty;
        }

        private string ObterCodicaoPAAI(FiltroRelatorioEncaminhamentoAeeDto filtro) =>
        	       !filtro.CodigosPAAIResponsavel.EstaFiltrandoTodas() ? " and coalesce(responsavel.login, responsavel.rf_codigo) = ANY(@codigosPAAIResponsavel) " : string.Empty;

        private string ObterCondicao(FiltroRelatorioEncaminhamentoAeeDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioEncaminhamentoAeeDto, string>>
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
    }
}
