using SME.SR.Infra;
using System.Runtime.InteropServices.ComTypes;

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

        internal static string DadosDreUe = @"select dre.id DreId, dre.abreviacao DreNome,
	 				ue.id UeId, concat(ue.ue_id, ' - ', tp.descricao, ' ', ue.nome) UeNome
					from  turma t
					inner join ue on ue.id = t.ue_id 
					inner join dre on ue.dre_id = dre.id 
					inner join tipo_escola tp on ue.tipo_escola = tp.id 
				   where t.turma_id = @codigoTurma";


		internal static string TurmaPorUe(Modalidade? modalidade, int? anoLetivo, int? semestre)
        {
			var query = @"select t.turma_id, t.nome, t.modalidade_codigo 
					from  turma t
					inner join ue on ue.id = t.ue_id
					where ue.ue_id = @codigoUe";

			if (modalidade.HasValue)
				query += " and t.modalidade_codigo = @modalidade";

			if (anoLetivo.HasValue)
				query += " and t.ano_letivo = @anoLetivo";

			if (semestre.HasValue)
				query += " and t.semestre = @anoLetivo";

			// TODO falta adicionar periodoEscolar

			return query;
        }

        internal static string TurmaPorCodigo = @"select t.turma_id CodigoTurma, 
			t.nome, t.modalidade_codigo  ModalidadeCodigo, t.semestre, t.ano, 
			t.ano_letivo AnoLetivo
        from  turma t
        where t.turma_id = @codigoTurma";
    }
}
