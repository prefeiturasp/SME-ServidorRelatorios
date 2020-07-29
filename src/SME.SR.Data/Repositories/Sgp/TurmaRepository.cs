using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public TurmaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<string> ObterCicloAprendizagem(string turmaCodigo)
        {
            var query = TurmaConsultas.CicloAprendizagemPorTurma;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                var ciclo = await conexao.QueryFirstOrDefaultAsync<string>(query, new { turmaCodigo });
                await conexao.CloseAsync();

                return ciclo;
            }
        }

        public async Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma)
        {
            var query = TurmaConsultas.DadosAlunos;
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Aluno>(query, parametros);
            }
        }
        public async Task<IEnumerable<Aluno>> ObterAlunosPorTurmas(IEnumerable<long> turmasId)
        {
            try
            {
                var query = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
						DROP TABLE #tmpAlunosFrequencia
					CREATE TABLE #tmpAlunosFrequencia 
					(
						CodigoAluno int,
						NomeAluno VARCHAR(70),
						DataNascimento DATETIME,
						NomeSocialAluno VARCHAR(70),
						CodigoSituacaoMatricula INT,
						SituacaoMatricula VARCHAR(40),
						DataSituacao DATETIME,
						NumeroAlunoChamada VARCHAR(5),
						PossuiDeficiencia BIT
					)
					INSERT INTO #tmpAlunosFrequencia
					SELECT aluno.cd_aluno CodigoAluno,
					   aluno.nm_aluno NomeAluno,
					   aluno.dt_nascimento_aluno DataNascimento,
					   aluno.nm_social_aluno NomeSocialAluno,
					   mte.cd_situacao_aluno CodigoSituacaoMatricula,
					   CASE
							WHEN mte.cd_situacao_aluno = 1 THEN 'Ativo'
							WHEN mte.cd_situacao_aluno = 2 THEN 'Desistente'
							WHEN mte.cd_situacao_aluno = 3 THEN 'Transferido'
							WHEN mte.cd_situacao_aluno = 4 THEN 'Vínculo Indevido'
							WHEN mte.cd_situacao_aluno = 5 THEN 'Concluído'
							WHEN mte.cd_situacao_aluno = 6 THEN 'Pendente de Rematrícula'
							WHEN mte.cd_situacao_aluno = 7 THEN 'Falecido'
							WHEN mte.cd_situacao_aluno = 8 THEN 'Não Compareceu'
							WHEN mte.cd_situacao_aluno = 10 THEN 'Rematriculado'
							WHEN mte.cd_situacao_aluno = 11 THEN 'Deslocamento'
							WHEN mte.cd_situacao_aluno = 12 THEN 'Cessado'
							WHEN mte.cd_situacao_aluno = 13 THEN 'Sem continuidade'
							WHEN mte.cd_situacao_aluno = 14 THEN 'Remanejado Saída'
							WHEN mte.cd_situacao_aluno = 15 THEN 'Reclassificado Saída'
							ELSE 'Fora do domínio liberado pela PRODAM'
							END SituacaoMatricula,
						mte.dt_situacao_aluno DataSituacao,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM v_aluno_cotic aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @turmasId
						UNION 
						SELECT  aluno.cd_aluno CodigoAluno,
						aluno.nm_aluno NomeAluno,
						aluno.dt_nascimento_aluno DataNascimento,
						aluno.nm_social_aluno NomeSocialAluno,
						mte.cd_situacao_aluno CodigoSituacaoMatricula,
						CASE
							WHEN mte.cd_situacao_aluno = 1 THEN 'Ativo'
							WHEN mte.cd_situacao_aluno = 2 THEN 'Desistente'
							WHEN mte.cd_situacao_aluno = 3 THEN 'Transferido'
							WHEN mte.cd_situacao_aluno = 4 THEN 'Vínculo Indevido'
							WHEN mte.cd_situacao_aluno = 5 THEN 'Concluído'
							WHEN mte.cd_situacao_aluno = 6 THEN 'Pendente de Rematrícula'
							WHEN mte.cd_situacao_aluno = 7 THEN 'Falecido'
							WHEN mte.cd_situacao_aluno = 8 THEN 'Não Compareceu'
							WHEN mte.cd_situacao_aluno = 10 THEN 'Rematriculado'
							WHEN mte.cd_situacao_aluno = 11 THEN 'Deslocamento'
							WHEN mte.cd_situacao_aluno = 12 THEN 'Cessado'
							WHEN mte.cd_situacao_aluno = 13 THEN 'Sem continuidade'
							WHEN mte.cd_situacao_aluno = 14 THEN 'Remanejado Saída'
							WHEN mte.cd_situacao_aluno = 15 THEN 'Reclassificado Saída'
							ELSE 'Fora do domínio liberado pela PRODAM'
							END SituacaoMatricula,
						mte.dt_situacao_aluno DataSituacao,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM v_aluno_cotic aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @turmasId
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola in @turmasId
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND mte.cd_turma_escola  in @turmasId) 

					SELECT
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					MAX(DataSituacao) DataSituacao ,
					NumeroAlunoChamada,
					PossuiDeficiencia
					FROM #tmpAlunosFrequencia
					GROUP BY
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					NumeroAlunoChamada,
					PossuiDeficiencia";
                var parametros = new { turmasId };

                using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
                return await conexao.QueryAsync<Aluno>(query, parametros);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<AlunoSituacaoDto>> ObterDadosAlunosSituacao(string turmaCodigo)
        {
            var query = TurmaConsultas.DadosAlunosSituacao;

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<AlunoSituacaoDto>(query, new { turmaCodigo });
            }
        }

        public async Task<DreUe> ObterDreUe(string codigoTurma)
        {
            var query = TurmaConsultas.DadosCompletosDreUe;
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<DreUe>(query, parametros);
            }
        }

        public async Task<Turma> ObterPorCodigo(string codigoTurma)
        {
            var query = @"select t.turma_id Codigo, t.nome, 
			                t.modalidade_codigo  ModalidadeCodigo, t.semestre, t.ano, t.ano_letivo AnoLetivo,
			                ue.id, ue.ue_id Codigo, ue.nome, ue.tipo_escola TipoEscola,		
			                dre.id, dre.dre_id Codigo, dre.abreviacao, dre.nome
			                from  turma t
			                inner join ue on ue.id = t.ue_id 
			                inner join dre on ue.dre_id = dre.id 
			                where t.turma_id = @codigoTurma";

            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<Turma, Ue, Dre, Turma>(query, (turma, ue, dre) =>
                {
                    turma.Dre = dre;
                    turma.Ue = ue;

                    return turma;
                }
                , parametros, splitOn: "Codigo,Id,Id")).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Turma>> ObterPorAbrangenciaFiltros(string codigoUe, Modalidade modalidade, int anoLetivo, string login, Guid perfil, bool consideraHistorico, int semestre, bool? possuiFechamento = null, bool? somenteEscolarizada = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"select ano, anoLetivo, codigo, 
								codigoModalidade modalidadeCodigo, nome, semestre 
							from f_abrangencia_turmas(@login, @perfil, @consideraHistorico, @modalidade, @semestre, @codigoUe, @anoLetivo)
                            where 1=1    ");


            if (possuiFechamento.HasValue)
                query.Append(@" and codigo in (select t.turma_id from fechamento_turma ft
                                 inner join turma t on ft.turma_id = t.id
                                 where not ft.excluido)");

            if (somenteEscolarizada.HasValue && somenteEscolarizada.Value)
                query.Append(" and ano != '0'");

            var parametros = new
            {
                CodigoUe = codigoUe,
                Modalidade = (int)modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Login = login,
                Perfil = perfil,
                ConsideraHistorico = consideraHistorico
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<Turma>(query.ToString(), parametros);
            }
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosEParecerConclusivo(long[] codigoAlunos, long[] codigoPareceresConclusivos)
        {
            var query = @"select distinct 
	                        t.turma_id as TurmaCodigo,
                            t.modalidade_codigo Modalidade,
	                        cca.aluno_codigo as AlunoCodigo,
	                        t.ano 
                        from
	                        fechamento_turma ft
                        inner join conselho_classe cc on
	                        cc.fechamento_turma_id = ft.id
                        inner join conselho_classe_aluno cca on
	                        cca.conselho_classe_id = cc.id
	                     inner join turma t 
	                     	on ft.turma_id = t.id
                        where
	                        cca.aluno_codigo = any(@codigoAlunos) 
	                        and cca.conselho_classe_parecer_id  = any(@codigoPareceresConclusivos)";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            var codigos = codigoAlunos.Select(a => a.ToString()).ToArray();

            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { codigoAlunos = codigos, codigoPareceresConclusivos });

        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaParecerConclusivo(long turmaCodigo, long[] codigoPareceresConclusivos)
        {
            try
            {


                var query = @"select distinct 
	                        ft.turma_id as TurmaCodigo,
	                        cca.aluno_codigo as AlunoCodigo,
	                        t.ano
                        from
	                        fechamento_turma ft
                        inner join conselho_classe cc on
	                        cc.fechamento_turma_id = ft.id
                        inner join conselho_classe_aluno cca on
	                        cca.conselho_classe_id = cc.id
	                     inner join turma t 
	                     	on ft.turma_id = t.id
                        where
	                       t.turma_id = @turmaCodigo
	                       and cca.conselho_classe_parecer_id = any(@codigoPareceresConclusivos)";


                using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

                return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { turmaCodigo = turmaCodigo.ToString(), codigoPareceresConclusivos });

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<IEnumerable<Turma>> ObterTurmasPorAno(int anoLetivo, string[] anosEscolares)
        {
            var query = @"select t.turma_id Codigo
                            , t.nome
                            , t.modalidade_codigo  ModalidadeCodigo
                            , t.semestre
                            , t.ano
                            , t.ano_letivo AnoLetivo
                        from turma t
                       where ano_letivo = @anoLetivo
                             and ano = ANY(@anosEscolares)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<Turma>(query, new { anoLetivo, anosEscolares });
            }

        }

        public async Task<IEnumerable<Turma>> ObterTurmasPorAnoEModalidade(int anoLetivo, string[] anosEscolares, Modalidade modalidade)
        {
            var query = new StringBuilder(@"select t.turma_id Codigo
                            , t.nome
                            , t.modalidade_codigo  ModalidadeCodigo
                            , t.semestre
                            , t.ano
                            , t.ano_letivo AnoLetivo
                        from turma t
                       where ano_letivo = @anoLetivo
                       and modalidade_codigo = @modalidade ");

            if (anosEscolares != null && anosEscolares.Any(c => c != "-99"))
                query.AppendLine("and ano = ANY(@anosEscolares)");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<Turma>(query.ToString(), new { anoLetivo, anosEscolares, modalidade });
            }

        }
    }
}
