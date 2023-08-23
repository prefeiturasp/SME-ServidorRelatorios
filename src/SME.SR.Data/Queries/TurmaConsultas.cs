using SME.SR.Infra;
using System.Security.Cryptography.Xml;

namespace SME.SR.Data
{
    public static class TurmaConsultas
    {
        internal static string DadosAlunos = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						WHERE mte.cd_turma_escola = @CodigoTurma
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						WHERE mte.cd_turma_escola = @CodigoTurma
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola = @CodigoTurma
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND mte.cd_turma_escola = @CodigoTurma) 

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


        internal static string DadosAlunosDataMatricula = @"with tmpAlunosDadosAtuais as (
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
														WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
														WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física' 
														ELSE 'Fora do domínio liberado pela PRODAM'
														END SituacaoMatricula,
													mte.dt_situacao_aluno DataSituacao,
													matr.dt_status_matricula DataMatricula,
													mte.nr_chamada_aluno NumeroAlunoChamada,
													CASE
														WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
														ELSE 1
													END PossuiDeficiencia,
													LTRIM(RTRIM(nm_responsavel)) NomeResponsavel,
													ra.tp_pessoa_responsavel TipoResponsavel,
													concat(LTRIM(RTRIM(ra.cd_ddd_celular_responsavel)), LTRIM(RTRIM(ra.nr_celular_responsavel))) CelularResponsavel,
													ra.dt_atualizacao_tabela DataAtualizacaoContato,
													matr.cd_matricula CodigoMatricula
												FROM v_aluno_cotic aluno
													INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
													INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
													LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
													LEFT JOIN responsavel_aluno ra ON aluno.cd_aluno = ra.cd_aluno and ra.dt_fim is null
												WHERE mte.cd_turma_escola = @CodigoTurma
													UNION
												SELECT
													aluno.cd_aluno CodigoAluno,
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
														WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
														WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física' 
														ELSE 'Fora do domínio liberado pela PRODAM'
														END SituacaoMatricula,
													mte.dt_situacao_aluno DataSituacao,
													matr.dt_status_matricula DataMatricula,
													mte.nr_chamada_aluno NumeroAlunoChamada,
													CASE WHEN nea.tp_necessidade_especial IS NULL
														THEN 0
														ELSE 1
													END PossuiDeficiencia,
													LTRIM(RTRIM(nm_responsavel)) NomeResponsavel,
													ra.tp_pessoa_responsavel TipoResponsavel,
													concat(LTRIM(RTRIM(ra.cd_ddd_celular_responsavel)), LTRIM(RTRIM(ra.nr_celular_responsavel))) CelularResponsavel,
													ra.dt_atualizacao_tabela DataAtualizacaoContato,
													matr.cd_matricula CodigoMatricula
												FROM v_aluno_cotic aluno
												INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
												INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
												INNER JOIN turma_escola te ON mte.cd_turma_escola = te.cd_turma_escola
												LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
												LEFT JOIN responsavel_aluno ra ON aluno.cd_aluno = ra.cd_aluno and ra.dt_fim is null
												WHERE mte.cd_turma_escola = @CodigoTurma
												and matr.an_letivo = te.an_letivo
								), lista as (
									select
									CodigoAluno,
									NomeAluno,
									NomeSocialAluno,
									DataNascimento,
									CodigoSituacaoMatricula,
									SituacaoMatricula,
									DataSituacao,
									NumeroAlunoChamada,
									PossuiDeficiencia,
									NomeResponsavel,
									TipoResponsavel,
									CelularResponsavel,
									DataAtualizacaoContato,
									CodigoMatricula,
									DataMatricula,
									row_number() over (partition by CodigoAluno order by DataSituacao desc) sequencia
									from tmpAlunosDadosAtuais
									where ((CONVERT(date, DataSituacao) <= CONVERT(date, @DataMatricula) and CodigoSituacaoMatricula not in (1,5,6,10,13)) 
									or (CONVERT(date, DataMatricula) <= CONVERT(date, @DataMatricula))))

							select * from lista
							where sequencia = 1
							and CodigoSituacaoMatricula <> 4
							order by NomeAluno;";

        internal static string DadosAlunosSituacao = @"
					IF OBJECT_ID('tempdb..#tmpAlunosSituacao') IS NOT NULL
						DROP TABLE #tmpAlunosSituacao

					CREATE TABLE #tmpAlunosSituacao 
					(
						CodigoAluno int,
						NomeAluno VARCHAR(70),
						CodigoSituacaoMatricula INT,
						SituacaoMatricula VARCHAR(40),
						NumeroAlunoChamada VARCHAR(5),
						DataSituacaoAluno DATETIME,
						DataMatricula DATETIME
					)

					IF OBJECT_ID('tempdb..#tmpMatriculas') IS NOT NULL
						DROP TABLE #tmpMatriculas
						select matr.cd_aluno matr_cd_aluno,
						matr.dt_status_matricula,
						mte.dt_situacao_aluno,
						matr.cd_matricula matr_cd_matricula
					into #tmpMatriculas
					from v_matricula_cotic matr 
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
					WHERE mte.cd_turma_escola = @turmaCodigo

					INSERT INTO #tmpAlunosSituacao
					SELECT aluno.cd_aluno CodigoAluno,
					   coalesce(aluno.nm_social_aluno,aluno.nm_aluno) NomeAluno,
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
							ELSE 'Fora do domínio liberado pela PRODAM'
							END SituacaoMatricula,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mte.dt_situacao_aluno DataSituacaoAluno,
						COALESCE(MIN(hmte.dt_situacao_aluno), MAX(hm.dt_status_matricula), MAX(matr.dt_status_matricula)) DataMatricula
					FROM v_aluno_cotic aluno
					INNER JOIN #tmpMatriculas TMT ON aluno.cd_aluno = TMT.matr_cd_aluno
					INNER JOIN v_matricula_cotic matr ON TMT.matr_cd_aluno = matr.cd_aluno and matr.cd_matricula = TMT.matr_cd_matricula
					INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
					LEFT JOIN historico_matricula_turma_escola hmte ON matr.cd_matricula = hmte.cd_matricula
					LEFT JOIN v_historico_matricula_cotic hm ON matr.cd_matricula = hm.cd_matricula and matr.an_letivo = hm.an_letivo and hm.st_matricula <> 4
					WHERE mte.cd_turma_escola = @turmaCodigo
					group by
					aluno.cd_aluno,
					coalesce(aluno.nm_social_aluno,aluno.nm_aluno),
					mte.cd_situacao_aluno,
					mte.nr_chamada_aluno,
					mte.dt_situacao_aluno


						UNION 

					SELECT  aluno.cd_aluno CodigoAluno,
					    coalesce(aluno.nm_social_aluno,aluno.nm_aluno) NomeAluno,
					    COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) CodigoSituacaoMatricula,
					    CASE
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 1 THEN 'Ativo'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 2 THEN 'Desistente'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 3 THEN 'Transferido'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 4 THEN 'Vínculo Indevido'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 5 THEN 'Concluído'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 6 THEN 'Pendente de Rematrícula'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 7 THEN 'Falecido'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 8 THEN 'Não Compareceu'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 10 THEN 'Rematriculado'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 11 THEN 'Deslocamento'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 12 THEN 'Cessado'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 13 THEN 'Sem continuidade'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 14 THEN 'Remanejado Saída'
						    WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 15 THEN 'Reclassificado Saída'
							WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 16 THEN 'Transferido SED'
							WHEN COALESCE(amn.CodigoSituacaoMatricula, mte.cd_situacao_aluno) = 17 THEN 'Dispensado Ed. Física'
						    ELSE 'Fora do domínio liberado pela PRODAM'
						    END SituacaoMatricula,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						COALESCE(amn.DataSituacao, mte.dt_situacao_aluno) DataSituacaoAluno,
						(select MIN(dt_situacao_aluno) from historico_matricula_turma_escola where cd_matricula = matr.cd_matricula) DataMatricula
					FROM v_aluno_cotic aluno
					INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
					INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
					LEFT JOIN alunos_matriculas_norm amn on amn.CodigoMatricula  = mte.cd_matricula 
					INNER JOIN turma_escola te ON mte.cd_turma_escola = te.cd_turma_escola
					WHERE mte.cd_turma_escola = @turmaCodigo and amn.CodigoTurma = @turmaCodigo
						and mte.dt_situacao_aluno =
							(select max(mte2.dt_situacao_aluno) 
							    from v_historico_matricula_cotic  matr2
								   INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							 where mte2.cd_turma_escola = @turmaCodigo
							   and matr2.cd_aluno = matr.cd_aluno)
					SELECT
					CodigoAluno,
					NomeAluno,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					NumeroAlunoChamada,
					DataSituacaoAluno,
					DataMatricula
					FROM #tmpAlunosSituacao
					GROUP BY
					CodigoAluno,
					NomeAluno,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					NumeroAlunoChamada,
					DataSituacaoAluno,
					DataMatricula
					order by NomeAluno";



        internal static string DadosDreUe = @"
					select 
						dre.abreviacao Dre,
	 					concat(ue.ue_id, ' - ', tp.descricao, ' ', ue.nome) Ue
					from  turma t
					inner join ue on ue.id = t.ue_id 
					inner join dre on ue.dre_id = dre.id 
					inner join tipo_escola tp on ue.tipo_escola = tp.id 
				   where t.turma_id = @codigoTurma";



		internal static string DadosCompletosDreUe = @"
					select
						dre.id DreId,
						dre.dre_id DreCodigo,
						dre.abreviacao DreNome,
						ue.id UeId,
						ue.ue_id UeCodigo,
						concat(ue.ue_id, ' - ', tp.descricao, ' ', ue.nome) UeNome
					from  turma t
					inner join ue on ue.id = t.ue_id 
					inner join dre on ue.dre_id = dre.id 
					inner join tipo_escola tp on ue.tipo_escola = tp.cod_tipo_escola_eol 
				   where t.turma_id = @codigoTurma";

		internal static string DadosCompletosDreUePorTurmaId = @"
					select
						dre.id DreId,
						dre.dre_id DreCodigo,
						dre.abreviacao DreNome,
						ue.id UeId,
						ue.ue_id UeCodigo,
						concat(ue.ue_id, ' - ', tp.descricao, ' ', ue.nome) UeNome
					from  turma t
					inner join ue on ue.id = t.ue_id 
					inner join dre on ue.dre_id = dre.id 
					inner join tipo_escola tp on ue.tipo_escola = tp.cod_tipo_escola_eol 
				   where t.id = @turmaId";

		internal static string TurmaPorCodigo = @"select t.turma_id Codigo, t.nome, 
			t.modalidade_codigo  ModalidadeCodigo, t.semestre, t.ano, t.ano_letivo AnoLetivo,
			ue.id, ue.ue_id Codigo, ue.nome, ue.tipo_escola TipoEscola,		
			dre.id, dre.dre_id Codigo, dre.abreviacao, dre.nome
			from  turma t
			inner join ue on ue.id = t.ue_id 
			inner join dre on ue.dre_id = dre.id 
			where t.turma_id = @codigoTurma";


		internal static string CicloAprendizagemPorTurma = @"select c.descricao
          from turma t
         inner join tipo_ciclo_ano a on a.modalidade = t.modalidade_codigo 
 							        and a.ano = t.ano
         inner join tipo_ciclo c on c.id = a.tipo_ciclo_id
        where t.turma_id = @turmaCodigo ";

		internal static string TurmaPorAbrangenciaFiltros = @"select ano, anoLetivo, codigo, 
								codigoModalidade modalidadeCodigo, nome, semestre 
							from f_abrangencia_turmas(@login, @perfil, @consideraHistorico, @modalidade, @semestre, @codigoUe, @anoLetivo) ";
    }
}
