using SME.SR.Infra;
using System;

namespace SME.SR.Data
{
    public static class AlunoConsultas
    {
        public static int[] CodigosSituacoesAlunoAtivo = new int[]
        {
			(int)SituacaoMatriculaAluno.Ativo,
            (int)SituacaoMatriculaAluno.Concluido,
            (int)SituacaoMatriculaAluno.PendenteRematricula,
			(int)SituacaoMatriculaAluno.Rematriculado,
			(int)SituacaoMatriculaAluno.SemContinuidade
        };

        internal static string AlunosPorTurma = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
						DROP TABLE #tmpAlunosFrequencia
					CREATE TABLE #tmpAlunosFrequencia 
					(
						CodigoTurma int,
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
					SELECT mte.cd_turma_escola CodigoTurma,
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
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM v_aluno_cotic aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @CodigosTurma
						UNION 
						SELECT  mte.cd_turma_escola CodigoTurma,
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
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM v_aluno_cotic aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @CodigosTurma
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola in @CodigosTurma
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND mte.cd_turma_escola in @CodigosTurma) 

					SELECT
					CodigoTurma,
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
					CodigoTurma,
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					NumeroAlunoChamada,
					PossuiDeficiencia";

        internal static string AlunosPorCodigoETurma = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
						DROP TABLE #tmpAlunosFrequencia
					CREATE TABLE #tmpAlunosFrequencia 
					(
						CodigoTurma int,
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
					SELECT mte.cd_turma_escola CodigoTurma,
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
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM v_aluno_cotic aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @codigosTurma and aluno.cd_aluno in @codigosAluno
						UNION 
						SELECT  mte.cd_turma_escola CodigoTurma,
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
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM v_aluno_cotic aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @codigosTurma and aluno.cd_aluno in @codigosAluno
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola in @codigosTurma and matr2.cd_aluno in @codigosAluno
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND mte.cd_turma_escola in @codigosTurma
							AND matr3.cd_aluno in @codigosAluno) 

					SELECT
					CodigoTurma,
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
					CodigoTurma,
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					NumeroAlunoChamada,
					PossuiDeficiencia";

        internal static string DadosAluno = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
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
						WHERE mte.cd_turma_escola = @codigoTurma and aluno.cd_aluno = @codigoAluno
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
						WHERE mte.cd_turma_escola = @codigoTurma and aluno.cd_aluno = @codigoAluno
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola = @codigoTurma and matr2.cd_aluno = @codigoAluno
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND mte.cd_turma_escola = @codigoTurma
							AND matr3.cd_aluno = @codigoAluno) 

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

        internal static string DatasMatriculaAlunoNaTurma = @"with lista as (
																select mte.dt_situacao_aluno
																	from v_matricula_cotic m
																		inner join matricula_turma_escola mte
																			on m.cd_matricula = mte.cd_matricula
																where m.cd_aluno = @codigoAluno and
																	mte.cd_turma_escola = @codigoTurma

																union

																select mte.dt_situacao_aluno
																	from v_historico_matricula_cotic m
																		inner join historico_matricula_turma_escola mte
																			on m.cd_matricula = mte.cd_matricula
																where m.cd_aluno = @codigoAluno and
																	mte.cd_turma_escola = @codigoTurma)
																select min(dt_situacao_aluno) data_matricula,
																	   max(dt_situacao_aluno) data_situacao
																	from lista";

        internal static string TotalDeAlunosAtivosPorPeriodo(string dreId, string ueId, DateTime dataInicio) =>
        $@"SELECT CodigoUe, AnoTurma, sum(totalAluno) QuantidadeAluno 
				FROM( 
				SELECT 	COUNT(distinct m.cd_aluno) totalAluno,
						se.sg_resumida_serie as AnoTurma,
						ue.cd_unidade_educacao as CodigoUe
				FROM v_matricula_cotic m
					INNER JOIN matricula_turma_escola mte
						ON m.cd_matricula = mte.cd_matricula	
					INNER JOIN turma_escola te
						ON mte.cd_turma_escola = te.cd_turma_escola
					INNER JOIN serie_turma_escola ste
						ON te.cd_turma_escola = ste.cd_turma_escola
					INNER JOIN serie_turma_grade stg
						ON ste.cd_serie_ensino = stg.cd_serie_ensino
					INNER JOIN serie_ensino se
						ON stg.cd_serie_ensino = se.cd_serie_ensino
					INNER JOIN etapa_ensino ee
						ON se.cd_etapa_ensino = ee.cd_etapa_ensino
					INNER JOIN v_cadastro_unidade_educacao ue
						ON te.cd_escola = ue.cd_unidade_educacao
					INNER JOIN alunos_matriculas_norm aln 
						ON aln.CodigoMatricula = m.cd_matricula
				WHERE te.an_letivo = @anoLetivo AND
					  te.cd_tipo_turma = 1 AND
					  ((mte.cd_situacao_aluno = 10 or (mte.cd_situacao_aluno in (1, 6, 13, 5) and CAST(aln.DataMatricula AS DATE) < @dataFim))
					  or (mte.cd_situacao_aluno not in (1, 6, 10, 13, 5) and CAST(aln.DataMatricula AS DATE) < @dataFim
					  {(dataInicio == null || dataInicio == DateTime.MinValue
                   ? "and mte.dt_situacao_aluno >= @dataFim))"
                   : "and(mte.dt_situacao_aluno > @dataFim or (mte.dt_situacao_aluno > @dataInicio and mte.dt_situacao_aluno <= @dataFim))))")}
					  and aln.AnoLetivo = anoLetivo
					  AND ee.cd_etapa_ensino in (@modalidades)
					  {(!string.IsNullOrWhiteSpace(dreId) ? " AND ue.cd_unidade_administrativa_referencia = @codigoDre" : string.Empty)}
				      {(!string.IsNullOrWhiteSpace(ueId) ? " AND ue.cd_unidade_educacao = @codigoUe" : string.Empty)}							  
				      GROUP BY se.sg_resumida_serie, ue.cd_unidade_educacao
				UNION

				SELECT 	COUNT(distinct matr.cd_aluno) totalAluno,
						se.sg_resumida_serie as AnoTurma,
						ue.cd_unidade_educacao as CodigoUe
				FROM
					v_aluno_cotic aluno
				INNER JOIN v_historico_matricula_cotic matr ON
					aluno.cd_aluno = matr.cd_aluno
				INNER JOIN historico_matricula_turma_escola mte ON
					matr.cd_matricula = mte.cd_matricula
				INNER JOIN turma_escola te
					ON mte.cd_turma_escola = te.cd_turma_escola
				INNER JOIN serie_turma_escola ste
					ON te.cd_turma_escola = ste.cd_turma_escola
				INNER JOIN serie_turma_grade stg
					ON ste.cd_serie_ensino = stg.cd_serie_ensino
				INNER JOIN serie_ensino se
					ON stg.cd_serie_ensino = se.cd_serie_ensino
				INNER JOIN etapa_ensino ee
					ON se.cd_etapa_ensino = ee.cd_etapa_ensino
				INNER JOIN v_cadastro_unidade_educacao ue
					ON te.cd_escola = ue.cd_unidade_educacao
				WHERE te.an_letivo = @anoLetivo AND
					  te.cd_tipo_turma = 1 AND
					  ee.cd_etapa_ensino in (@modalidades)
				      AND mte.nr_chamada_aluno <> '0'
					  AND mte.nr_chamada_aluno is not null
                      AND mte.cd_situacao_aluno in (5,10)
                        AND NOT EXISTS(
					    SELECT
						    1
					    FROM
						    v_matricula_cotic matr3
					    INNER JOIN matricula_turma_escola mte3 ON
						    matr3.cd_matricula = mte3.cd_matricula
					    WHERE
						    mte.cd_matricula = mte3.cd_matricula and mte.cd_turma_escola = mte3.cd_turma_escola 
                            and matr3.cd_matricula = matr.cd_matricula and matr3.an_letivo = matr.an_letivo)
					  {(!string.IsNullOrWhiteSpace(dreId) ? " AND ue.cd_unidade_administrativa_referencia = @codigoDre" : string.Empty)}
					  {(!string.IsNullOrWhiteSpace(ueId) ? " AND ue.cd_unidade_educacao = @codigoUe" : string.Empty)}						  
						GROUP BY se.sg_resumida_serie, ue.cd_unidade_educacao) tab
						GROUP BY CodigoUe, AnoTurma
						ORDER BY CodigoUe, AnoTurma";


        internal static string AlunosAtivosPorTurmaEPeriodo = @"SELECT DISTINCT
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
					coalesce(mte.nr_chamada_aluno,'0') NumeroAlunoChamada
				FROM
					v_aluno_cotic aluno
				INNER JOIN v_matricula_cotic matr ON
					aluno.cd_aluno = matr.cd_aluno
				INNER JOIN matricula_turma_escola mte ON
					matr.cd_matricula = mte.cd_matricula
				INNER JOIN matricula_norm mn ON
					matr.cd_matricula = mn.matr_cd_matricula
				LEFT JOIN necessidade_especial_aluno nea ON
					nea.cd_aluno = matr.cd_aluno
				WHERE
					mte.cd_turma_escola = @codigoTurma
					and ((mte.cd_situacao_aluno in @codigosSituacoesAlunoAtivo and convert(date, mn.dt_status_matricula) < @dataReferenciaFim)
					or (mte.cd_situacao_aluno not in @codigosSituacoesAlunoAtivo and convert(date, mn.dt_status_matricula) < @dataReferenciaFim
					and convert(date, mte.dt_situacao_aluno) >= @dataReferenciaInicio))
				UNION
				SELECT DISTINCT
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
					mte.nr_chamada_aluno NumeroAlunoChamada
				FROM
					v_aluno_cotic aluno
				INNER JOIN v_historico_matricula_cotic matr ON
					aluno.cd_aluno = matr.cd_aluno
				INNER JOIN historico_matricula_turma_escola mte ON
					matr.cd_matricula = mte.cd_matricula
				INNER JOIN matricula_norm mn ON
					matr.cd_matricula = mn.matr_cd_matricula
				LEFT JOIN necessidade_especial_aluno nea ON
					nea.cd_aluno = matr.cd_aluno
				WHERE
					mte.cd_turma_escola = @codigoTurma
					and mte.nr_chamada_aluno <> '0'
					and mte.nr_chamada_aluno is not null
					and mte.dt_situacao_aluno = (
					select
						max(mte2.dt_situacao_aluno)
					from
						v_historico_matricula_cotic matr2
					INNER JOIN historico_matricula_turma_escola mte2 ON
						matr2.cd_matricula = mte2.cd_matricula
					where
						mte2.cd_turma_escola = @codigoTurma
						and matr2.cd_aluno = matr.cd_aluno
						and ((mte.cd_situacao_aluno in @codigosSituacoesAlunoAtivo and convert(date, mn.dt_status_matricula) < @dataReferenciaFim)
						or (mte.cd_situacao_aluno not in @codigosSituacoesAlunoAtivo and convert(date, mn.dt_status_matricula) < @dataReferenciaFim
					    and convert(date, mte.dt_situacao_aluno) >= @dataReferenciaInicio)))
					AND NOT EXISTS(
					SELECT
						1
					FROM
						v_matricula_cotic matr3
					INNER JOIN matricula_turma_escola mte3 ON
						matr3.cd_matricula = mte3.cd_matricula
					WHERE
						mte.cd_matricula = mte3.cd_matricula
						AND mte.cd_turma_escola = @codigoTurma )";

        internal static string AlunosMatriculasPorTurmas = @"with lista as (
																select mte.nr_chamada_aluno,
																	   a.nm_aluno,
																	   a.nm_social_aluno,
																	   mte.cd_turma_escola,
																	   m.cd_aluno,
																	   mte.dt_situacao_aluno,
																	   mte.cd_situacao_aluno
																	from v_matricula_cotic m
																		inner join matricula_turma_escola mte
																			on m.cd_matricula = mte.cd_matricula
																		inner join v_aluno_cotic a
																			on m.cd_aluno = a.cd_aluno
																where mte.cd_turma_escola in @codigosTurmas

																union

																select mte.nr_chamada_aluno,
																	   a.nm_aluno,
																	   a.nm_social_aluno,
																	   mte.cd_turma_escola,
																	   m.cd_aluno,
																	   mte.dt_situacao_aluno,
																	   mte.cd_situacao_aluno
																	from v_historico_matricula_cotic m
																		inner join historico_matricula_turma_escola mte
																			on m.cd_matricula = mte.cd_matricula
																		inner join v_aluno_cotic a
																			on m.cd_aluno = a.cd_aluno
																where mte.cd_turma_escola in @codigosTurmas)
																select distinct
																	   (select top 1 nr_chamada_aluno 
																			from lista 
																		where cd_aluno = l.cd_aluno and 
																			  cd_turma_escola = l.cd_turma_escola 
																		order by dt_situacao_aluno desc) NumeroChamada,
																	   nm_aluno Nome,
																	   (select top 1 nm_social_aluno 
																			from lista 
																		where cd_aluno = l.cd_aluno and 
																			  cd_turma_escola = l.cd_turma_escola 
																		order by dt_situacao_aluno desc) NomeFinal,
																	   cd_turma_escola TurmaCodigo,
																	   cd_aluno CodigoAluno,
																	   (select top 1 dt_situacao_aluno 
																			from lista 
																		where cd_aluno = l.cd_aluno and 
																			  cd_turma_escola = l.cd_turma_escola 
																		order by dt_situacao_aluno) DataMatricula,
																	   (select top 1 dt_situacao_aluno 
																			from lista 
																		where cd_aluno = l.cd_aluno and 
																			  cd_turma_escola = l.cd_turma_escola 
																		order by dt_situacao_aluno desc) DataSituacao,
																	   (select top 1 cd_situacao_aluno 
																			from lista 
																		where cd_aluno = l.cd_aluno and 
																			  cd_turma_escola = l.cd_turma_escola 
																		order by dt_situacao_aluno desc) SituacaoMatricula
																	from lista l
																group by nr_chamada_aluno,
																		 nm_aluno,
																		 nm_social_aluno,
																		 cd_turma_escola,
																		 cd_aluno";

    }
}
