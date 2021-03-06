﻿using Dapper;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<Aluno>> ObterAlunosPorTurmaDataSituacaoMaricula(long turmaCodigo, DateTime dataReferencia)
        {
            var query = @"
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
						ELSE 'Fora do domínio liberado pela PRODAM'
					END SituacaoMatricula,
					mte.dt_situacao_aluno DataSituacao,
					mte.nr_chamada_aluno NumeroAlunoChamada
				FROM
					v_aluno_cotic aluno
				INNER JOIN v_matricula_cotic matr ON
					aluno.cd_aluno = matr.cd_aluno
				INNER JOIN matricula_turma_escola mte ON
					matr.cd_matricula = mte.cd_matricula
				LEFT JOIN necessidade_especial_aluno nea ON
					nea.cd_aluno = matr.cd_aluno
				WHERE
					mte.cd_turma_escola = @turmaCodigo
					and mte.nr_chamada_aluno <> 0
					and mte.nr_chamada_aluno is not null
					and (matr.st_matricula in (1, 6, 10, 13, 5)
					or (matr.st_matricula not in (1, 6, 10, 13, 5)
					and matr.dt_status_matricula > @dataReferencia))
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
				LEFT JOIN necessidade_especial_aluno nea ON
					nea.cd_aluno = matr.cd_aluno
				WHERE
					mte.cd_turma_escola = @turmaCodigo
					and mte.nr_chamada_aluno <> 0
					and mte.nr_chamada_aluno is not null
					and mte.dt_situacao_aluno = (
					select
						max(mte2.dt_situacao_aluno)
					from
						v_historico_matricula_cotic matr2
					INNER JOIN historico_matricula_turma_escola mte2 ON
						matr2.cd_matricula = mte2.cd_matricula
					where
						mte2.cd_turma_escola = @turmaCodigo
						and matr2.cd_aluno = matr.cd_aluno
						and (matr2.st_matricula in (1, 6, 10, 13, 5)
						or (matr2.st_matricula not in (1, 6, 10, 13, 5)
						and matr2.dt_status_matricula > @dataReferencia)) )
					AND NOT EXISTS(
					SELECT
						1
					FROM
						v_matricula_cotic matr3
					INNER JOIN matricula_turma_escola mte3 ON
						matr3.cd_matricula = mte3.cd_matricula
					WHERE
						mte.cd_matricula = mte3.cd_matricula
						AND mte.cd_turma_escola = @turmaCodigo)";

            var parametros = new { turmaCodigo, dataReferencia };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryAsync<Aluno>(query, parametros);

        }
        public async Task<int> ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(string anoTurma, string ueCodigo, int anoLetivo, long dreCodigo, DateTime dataReferencia)
        {
            StringBuilder query = new StringBuilder();
            if (anoLetivo < DateTime.Now.Date.Year)
            {
                query.AppendLine(@"
					SELECT
	                    count(DISTINCT matricula.cd_aluno) Total
                    FROM
	                    v_historico_matricula_cotic matricula
                    left JOIN historico_matricula_turma_escola matrTurma ON
	                    matricula.cd_matricula = matrTurma.cd_matricula and matrTurma.nr_chamada_aluno is not null
                    INNER JOIN turma_escola turesc ON
	                    matrTurma.cd_turma_escola = turesc.cd_turma_escola
                    INNER JOIN v_cadastro_unidade_educacao vue ON
	                    vue.cd_unidade_educacao = turesc.cd_escola
                    INNER JOIN (
	                    SELECT
		                    v_ua.cd_unidade_educacao, v_ua.nm_unidade_educacao, v_ua.nm_exibicao_unidade
	                    FROM
		                    unidade_administrativa ua
	                    INNER JOIN v_cadastro_unidade_educacao v_ua ON
		                    v_ua.cd_unidade_educacao = ua.cd_unidade_administrativa
	                    WHERE
		                    tp_unidade_administrativa = 24) dre ON
	                    dre.cd_unidade_educacao = vue.cd_unidade_administrativa_referencia
	                    --Serie Ensino
                    left join serie_turma_escola ste ON
	                    ste.cd_turma_escola = turesc.cd_turma_escola
                    left join serie_turma_grade ON
	                    serie_turma_grade.cd_turma_escola = ste.cd_turma_escola
                    left join escola_grade ON
	                    serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                    left join grade ON
	                    escola_grade.cd_grade = grade.cd_grade
                    left join serie_ensino se ON
	                    grade.cd_serie_ensino = se.cd_serie_ensino
                    where
						matrTurma.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola = matrTurma.cd_turma_escola
							and matr2.cd_aluno = matricula.cd_aluno
							and (matr2.st_matricula in (1, 6, 10, 13, 5) or (matr2.st_matricula not in (1, 6, 10, 13, 5) and matr2.dt_status_matricula > @dataFim))
						)
	                    and turesc.an_letivo = @anoLetivo
	                    and turesc.cd_tipo_turma = 1
						and se.sg_resumida_serie = @anoTurma
	                    and ( matricula.st_matricula in (1, 6, 10, 13, 5)   or  (matricula.st_matricula not in (1, 6, 10, 13, 5) 
						and matricula.dt_status_matricula > @dataFim)  ) ");

                if (!string.IsNullOrEmpty(ueCodigo))
                    query.Append("and vue.cd_unidade_educacao = @ueCodigo ");

                if (dreCodigo > 0)
                    query.Append("and dre.cd_unidade_educacao = @dreCodigo ");

                if (!string.IsNullOrEmpty(anoTurma))
                    query.Append("and se.sg_resumida_serie = @anoTurma ");

                query.AppendLine(" and se.cd_etapa_ensino in (5, 13)");
            }
            else
            {
                query.AppendLine(@"
					SELECT
	                    count(DISTINCT matricula.cd_aluno) Total
                    FROM
	                    v_matricula_cotic matricula
                    left JOIN matricula_turma_escola matrTurma ON
	                    matricula.cd_matricula = matrTurma.cd_matricula and matrTurma.nr_chamada_aluno is not null
                    INNER JOIN turma_escola turesc ON
	                    matrTurma.cd_turma_escola = turesc.cd_turma_escola
                    INNER JOIN v_cadastro_unidade_educacao vue ON
	                    vue.cd_unidade_educacao = turesc.cd_escola
                    INNER JOIN (
	                    SELECT
		                    v_ua.cd_unidade_educacao, v_ua.nm_unidade_educacao, v_ua.nm_exibicao_unidade
	                    FROM
		                    unidade_administrativa ua
	                    INNER JOIN v_cadastro_unidade_educacao v_ua ON
		                    v_ua.cd_unidade_educacao = ua.cd_unidade_administrativa
	                    WHERE
		                    tp_unidade_administrativa = 24) dre ON
	                    dre.cd_unidade_educacao = vue.cd_unidade_administrativa_referencia
	                    --Serie Ensino
                    left join serie_turma_escola ste ON
	                    ste.cd_turma_escola = turesc.cd_turma_escola
                    left join serie_turma_grade ON
	                    serie_turma_grade.cd_turma_escola = ste.cd_turma_escola
                    left join escola_grade ON
	                    serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                    left join grade ON
	                    escola_grade.cd_grade = grade.cd_grade
                    left join serie_ensino se ON
	                    grade.cd_serie_ensino = se.cd_serie_ensino
                    where
	                  	turesc.an_letivo = @anoLetivo
	                    and turesc.cd_tipo_turma = 1
	                    and ( matricula.st_matricula in (1, 6, 10, 13, 5)   or  (matricula.st_matricula not in (1, 6, 10, 13, 5) and matricula.dt_status_matricula > @dataFim)  ) 
	                    and se.cd_etapa_ensino in (5, 13)");

                if (!string.IsNullOrEmpty(ueCodigo))
                    query.Append("and vue.cd_unidade_educacao = @ueCodigo ");

                if (dreCodigo > 0)
                    query.Append("and dre.cd_unidade_educacao = @dreCodigo ");

                if (!string.IsNullOrEmpty(anoTurma))
                    query.Append("and se.sg_resumida_serie = @anoTurma ");
            }

            var parametros = new { dreCodigo, ueCodigo, anoTurma, anoLetivo, dataFim = dataReferencia };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryFirstOrDefaultAsync<int>(query.ToString(), parametros, commandTimeout: 600);
        }

        public async Task<Aluno> ObterDados(string codigoTurma, string codigoAluno)
        {
            var query = AlunoConsultas.DadosAluno;
            var parametros = new { CodigoTurma = codigoTurma, CodigoAluno = codigoAluno };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryFirstOrDefaultAsync<Aluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunoHistoricoEscolar(long[] codigosAlunos)
        {
            var query = @"IF OBJECT_ID('tempdb..#tmpAlunosHistoricoEscolar') IS NOT NULL
						DROP TABLE #tmpAlunosHistoricoEscolar
					CREATE TABLE #tmpAlunosHistoricoEscolar 
					(
						CodigoAluno int,
						NomeAluno VARCHAR(70),
						DataNascimento DATETIME,
						NomeSocialAluno VARCHAR(70),
						CodigoSituacaoMatricula INT,
						SituacaoMatricula VARCHAR(40),
						DataSituacao DATETIME,
						AnoLetivo SMALLINT,
						NumeroAlunoChamada VARCHAR(5),
						CidadeNatal VARCHAR(40),
						EstadoNatal CHAR(2),
						Nacionalidade VARCHAR(20),
						RG VARCHAR(30),
						ExpedicaoOrgaoEmissor VARCHAR(10),
						ExpedicaoUF VARCHAR(2),
						ExpedicaoData DATETIME,
						PossuiDeficiencia BIT
					)
					INSERT INTO #tmpAlunosHistoricoEscolar
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
						matr.an_letivo AnoLetivo,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_registro_aluno_estado EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in @codigosAluno
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
						matr.an_letivo AnoLetivo,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_registro_aluno_estado EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in @codigosAluno
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
								matr2.cd_aluno in @codigosAluno
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND matr3.cd_aluno in @codigosAluno) 
					SELECT
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,		
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					MAX(DataSituacao) DataSituacao,		
					AnoLetivo,
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada
					FROM #tmpAlunosHistoricoEscolar
					GROUP BY
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					AnoLetivo,
					DataNascimento,					
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada";

            var parametros = new { CodigosAluno = codigosAlunos };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoHistoricoEscolar>(query, parametros);

        }

        public async Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigos(long[] codigosAlunos)
        {
            var query = @"IF OBJECT_ID('tempdb..#tmpAlunosPorCodigo') IS NOT NULL
						DROP TABLE #tmpAlunosPorCodigo
					CREATE TABLE #tmpAlunosPorCodigo 
					(
						CodigoAluno int,
						NomeAluno VARCHAR(70),
						DataNascimento DATETIME,
						NomeSocialAluno VARCHAR(70),
						CodigoSituacaoMatricula INT,
						SituacaoMatricula VARCHAR(40),
						DataSituacao DATETIME,
						AnoLetivo SMALLINT,
						CodigoTurma INT,
						CodigoEscola CHAR(6),
						NumeroAlunoChamada VARCHAR(5),
						CidadeNatal VARCHAR(40),
						EstadoNatal CHAR(2),
						Nacionalidade VARCHAR(20),
						RG VARCHAR(30),
						ExpedicaoOrgaoEmissor VARCHAR(10),
						ExpedicaoUF VARCHAR(2),
						ExpedicaoData DATETIME,
						PossuiDeficiencia BIT
					)
					INSERT INTO #tmpAlunosPorCodigo
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
						matr.an_letivo AnoLetivo,
						mte.cd_turma_escola CodigoTurma,
						matr.cd_escola CodigoEscola,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_nascimento_aluno EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in (#codigosAlunos)
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
						matr.an_letivo AnoLetivo,
						mte.cd_turma_escola CodigoTurma,
						matr.cd_escola CodigoEscola,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_nascimento_aluno EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in (#codigosAlunos)
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
								matr2.cd_aluno in (#codigosAlunos)
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND matr3.cd_aluno in (#codigosAlunos)) 
					SELECT
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,		
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					MAX(DataSituacao) DataSituacao,	
					AnoLetivo,
					CodigoTurma,
					CodigoEscola,
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada
					FROM #tmpAlunosPorCodigo
					GROUP BY
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					AnoLetivo,
					CodigoTurma,
					CodigoEscola,
					DataNascimento,					
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoHistoricoEscolar>(query.Replace("#codigosAlunos", string.Join(" ,", codigosAlunos)), commandTimeout: 60);
        }

		public async Task<IEnumerable<Aluno>> ObterPorCodigosAlunoETurma(string[] codigosTurma, string[] codigosAluno)
		{
			var query = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
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
							ELSE 'Fora do domínio liberado pela PRODAM'
							END SituacaoMatricula,
						mte.dt_situacao_aluno DataSituacao,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
                      FROM matricula_turma_escola mte1 
                     INNER JOIN v_matricula_cotic matr ON matr.cd_matricula = mte1.cd_matricula 
                     INNER JOIN v_aluno_cotic aluno ON aluno.cd_aluno = matr.cd_aluno 
                     inner join matricula_turma_escola mte on mte.cd_matricula = mte1.cd_matricula 
                      LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte1.cd_turma_escola in @codigosTurma and aluno.cd_aluno in @codigosAluno
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
							ELSE 'Fora do domínio liberado pela PRODAM'
							END SituacaoMatricula,
						mte.dt_situacao_aluno DataSituacao,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
                    FROM v_historico_matricula_cotic matr
                    INNER JOIN v_aluno_cotic aluno ON aluno.cd_aluno = matr.cd_aluno
                    INNER JOIN historico_matricula_turma_escola mte1 ON matr.cd_matricula = mte1.cd_matricula
                    INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
                     LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte1.cd_turma_escola in @codigosTurma and aluno.cd_aluno in @codigosAluno
						and mte1.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
							mte2.cd_turma_escola in @codigosTurma and matr2.cd_aluno in @codigosAluno
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte1.cd_matricula = mte3.cd_matricula
							AND mte1.cd_turma_escola in @codigosTurma
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
					PossuiDeficiencia
					ORDER BY CodigoSituacaoMatricula";

			var parametros = new { CodigosTurma = codigosTurma, CodigosAluno = codigosAluno };

			using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
			{
				return await conexao.QueryAsync<Aluno>(query, parametros);
			}
		}

		public async Task<IEnumerable<Aluno>> ObterPorCodigosTurma(string[] codigosTurma)
		{
			var query = @"IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
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
		                    ELSE 'Fora do domínio liberado pela PRODAM'
		                    END SituacaoMatricula,
	                    mte.dt_situacao_aluno DataSituacao,
	                    mte.nr_chamada_aluno NumeroAlunoChamada,
	                    CASE
		                    WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
		                    ELSE 1
	                    END PossuiDeficiencia
                      FROM matricula_turma_escola mte1 
                     INNER JOIN v_matricula_cotic matr ON matr.cd_matricula = mte1.cd_matricula 
                     INNER JOIN v_aluno_cotic aluno ON aluno.cd_aluno = matr.cd_aluno 
                     inner join matricula_turma_escola mte on mte.cd_matricula = mte1.cd_matricula 
                      LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
                      WHERE mte1.cd_turma_escola in @CodigosTurma
                   UNION 
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
		                    ELSE 'Fora do domínio liberado pela PRODAM'
		                    END SituacaoMatricula,
	                    mte.dt_situacao_aluno DataSituacao,
	                    mte.nr_chamada_aluno NumeroAlunoChamada,
	                    CASE
		                    WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
		                    ELSE 1
	                    END PossuiDeficiencia
                    FROM v_historico_matricula_cotic matr
                    INNER JOIN v_aluno_cotic aluno ON aluno.cd_aluno = matr.cd_aluno
                    INNER JOIN historico_matricula_turma_escola mte1 ON matr.cd_matricula = mte1.cd_matricula
                    INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
                     LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
                    WHERE mte1.cd_turma_escola in @CodigosTurma
	                    and mte1.dt_situacao_aluno =                    
	                    (
	                     select max(mte2.dt_situacao_aluno) 
	                       from v_historico_matricula_cotic  matr2
	                      INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
	                    where
		                    mte2.cd_turma_escola in @CodigosTurma
	                    and matr2.cd_aluno = matr.cd_aluno
	                    )
	                    AND NOT EXISTS(
		                    SELECT 1 FROM v_matricula_cotic matr3
	                    INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
	                    WHERE mte1.cd_matricula = mte3.cd_matricula
		                    AND mte1.cd_turma_escola in @CodigosTurma) 

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
					PossuiDeficiencia
					ORDER BY CodigoSituacaoMatricula";

			var parametros = new { CodigosTurma = codigosTurma };

			using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
			{
				return await conexao.QueryAsync<Aluno>(query, parametros);
			}
		}

		public async Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosHistoricoAlunosPorCodigos(long[] codigosAlunos)
        {
            var query = @"IF OBJECT_ID('tempdb..#tmpAlunosPorCodigo') IS NOT NULL
						DROP TABLE #tmpAlunosPorCodigo
					CREATE TABLE #tmpAlunosPorCodigo 
					(
						CodigoAluno int,
						NomeAluno VARCHAR(70),
						DataNascimento DATETIME,
						NomeSocialAluno VARCHAR(70),
						CodigoSituacaoMatricula INT,
						SituacaoMatricula VARCHAR(40),
						DataSituacao DATETIME,
						AnoLetivo SMALLINT,
						CodigoTurma INT,
						CodigoEscola CHAR(6),
						NumeroAlunoChamada VARCHAR(5),
						CidadeNatal VARCHAR(40),
						EstadoNatal CHAR(2),
						Nacionalidade VARCHAR(20),
						RG VARCHAR(30),
						ExpedicaoOrgaoEmissor VARCHAR(10),
						ExpedicaoUF VARCHAR(2),
						ExpedicaoData DATETIME,
						PossuiDeficiencia BIT
					)
					INSERT INTO #tmpAlunosPorCodigo
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
						matr.an_letivo AnoLetivo,
						mte.cd_turma_escola CodigoTurma,
						matr.cd_escola CodigoEscola,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_registro_aluno_estado EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in @codigosAluno
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
						matr.an_letivo AnoLetivo,
						mte.cd_turma_escola CodigoTurma,
						matr.cd_escola CodigoEscola,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_registro_aluno_estado EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in @codigosAluno
						and mte.dt_situacao_aluno in                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
								matr2.cd_aluno in @codigosAluno
							and matr2.cd_aluno = matr.cd_aluno
							group by cd_turma_escola
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND matr3.cd_aluno in @codigosAluno) 
					SELECT
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,		
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					MAX(DataSituacao) DataSituacao,	
					AnoLetivo,
					CodigoTurma,
					CodigoEscola,
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada
					FROM #tmpAlunosPorCodigo
					GROUP BY
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					AnoLetivo,
					CodigoTurma,
					CodigoEscola,
					DataNascimento,					
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada";

            var parametros = new { CodigosAluno = codigosAlunos };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoHistoricoEscolar>(query, parametros, commandTimeout: 6000);


        }

        public async Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigosEAnoLetivo(long[] codigosAlunos, long anoLetivo)
        {
            var query = @"IF OBJECT_ID('tempdb..#tmpAlunosPorCodigo') IS NOT NULL
						DROP TABLE #tmpAlunosPorCodigo
					CREATE TABLE #tmpAlunosPorCodigo 
					(
						CodigoAluno int,
						NomeAluno VARCHAR(70),
						DataNascimento DATETIME,
						NomeSocialAluno VARCHAR(70),
						CodigoSituacaoMatricula INT,
						SituacaoMatricula VARCHAR(40),
						DataSituacao DATETIME,
						AnoLetivo SMALLINT,
						CodigoTurma INT,
						CodigoEscola CHAR(6),
						NumeroAlunoChamada VARCHAR(5),
						CidadeNatal VARCHAR(40),
						EstadoNatal CHAR(2),
						Nacionalidade VARCHAR(20),
						RG VARCHAR(30),
						ExpedicaoOrgaoEmissor VARCHAR(10),
						ExpedicaoUF VARCHAR(2),
						ExpedicaoData DATETIME,
						PossuiDeficiencia BIT
					)
					INSERT INTO #tmpAlunosPorCodigo
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
						matr.an_letivo AnoLetivo,
						mte.cd_turma_escola CodigoTurma,
						matr.cd_escola CodigoEscola,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_registro_aluno_estado EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in @codigosAluno 
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
						matr.an_letivo AnoLetivo,
						mte.cd_turma_escola CodigoTurma,
						matr.cd_escola CodigoEscola,
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mun.nm_municipio CidadeNatal,
						aluno.sg_uf_registro_aluno_estado EstadoNatal,
						CASE
							WHEN aluno.cd_nacionalidade_aluno = 'B' THEN 'Brasileira'
							ELSE 'Estrangeira'
						END Nacionalidade,
						aluno.nr_rg_aluno + '-' + aluno.cd_digito_rg_aluno RG,
						orge.cd_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE aluno.cd_aluno in @codigosAluno 
						and mte.dt_situacao_aluno =                    
							(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
							INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							where
								matr2.cd_aluno in @codigosAluno
							and matr2.cd_aluno = matr.cd_aluno
						)
						AND NOT EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
						INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						WHERE mte.cd_matricula = mte3.cd_matricula
							AND matr3.cd_aluno in @codigosAluno) 
					SELECT
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					DataNascimento,		
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					MAX(DataSituacao) DataSituacao,	
					AnoLetivo,
					CodigoTurma,
					CodigoEscola,
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada
					FROM #tmpAlunosPorCodigo
					GROUP BY
					CodigoAluno,
					NomeAluno,
					NomeSocialAluno,
					CodigoSituacaoMatricula,
					SituacaoMatricula,
					AnoLetivo,
					CodigoTurma,
					CodigoEscola,
					DataNascimento,					
					CidadeNatal,
					EstadoNatal,
					Nacionalidade,
					RG,
					ExpedicaoOrgaoEmissor,
					ExpedicaoUF,
					ExpedicaoData,
					PossuiDeficiencia,
                    NumeroAlunoChamada";

            var parametros = new { CodigosAluno = codigosAlunos, anoLetivo };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoHistoricoEscolar>(query, parametros);

        }

        public async Task<IEnumerable<AlunoResponsavelAdesaoAEDto>> ObterAlunosResponsaveisPorTurmasCodigoParaRelatorioAdesao(long[] turmasCodigo, int anoLetivo)
        {

            var query = @"select 
						distinct
						te.cd_turma_escola TurmaCodigo,
						a.cd_aluno AlunoCodigo,
						a.nm_aluno AlunoNome,
						a.nm_social_aluno AlunoNomeSocial,
						mte.nr_chamada_aluno AlunoNumeroChamada,
						ra.cd_cpf_responsavel as ResponsavelCpf,
						ra.nm_responsavel as ResponsavelNome,
						ra.cd_ddd_celular_responsavel as ResponsavelDDD,
						ra.nr_celular_responsavel as ResponsavelCelular,						
						coalesce(ra.cd_cpf_responsavel, 0) CpfResponsavel
					from v_aluno_cotic(nolock) a
					inner join responsavel_aluno(nolock) ra on ra.cd_aluno = a.cd_aluno
					inner join v_matricula_cotic(nolock) m on m.cd_aluno = a.cd_aluno
					inner join matricula_turma_escola(nolock) mte on mte.cd_matricula = m.cd_matricula
					inner join v_cadastro_unidade_educacao(nolock) vue on vue.cd_unidade_educacao = m.cd_escola
					inner join v_cadastro_unidade_educacao(nolock) dre on dre.cd_unidade_educacao = vue.cd_unidade_administrativa_referencia
					inner join turma_escola(nolock) te on te.cd_turma_escola = mte.cd_turma_escola
					where
						mte.cd_situacao_aluno IN( 1, 6, 10, 13 ) and
						ra.dt_fim IS NULL and te.cd_turma_escola in @turmasCodigo
						and m.an_letivo = @anoLetivo";

            var parametros = new { turmasCodigo, anoLetivo };

			using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
			{
				return await conexao.QueryAsync<AlunoResponsavelAdesaoAEDto>(query, parametros);
			}
		}

        public async Task<IEnumerable<AlunoNomeDto>> ObterNomesAlunosPorCodigos(string[] codigos)
        {
            var query = @"select vac.cd_aluno as Codigo, vac.nm_aluno as Nome
                          from v_aluno_cotic vac  
                          where vac.cd_aluno in @codigos";

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<AlunoNomeDto>(query, new { codigos });
            }
        }

		public async Task<IEnumerable<AlunoReduzidoDto>> ObterAlunosReduzidosPorTurmaEAluno(long turmaCodigo, long? alunoCodigo)
		{
			var query = @$"SELECT distinct aluno.cd_aluno AlunoCodigo,
										   aluno.nm_aluno NomeAluno,
										   aluno.nm_social_aluno NomeSocialAluno,					   
										   mte.cd_turma_escola TurmaCodigo                       
									  FROM v_aluno_cotic aluno
									 INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
									 INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
									  LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno				  
									 WHERE mte.cd_turma_escola = @turmaCodigo
									   {(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : "AND mte.cd_situacao_aluno in (1,5,6,10,13)")}
							 UNION 
							 SELECT aluno.cd_aluno AlunoCodigo,
									aluno.nm_aluno NomeAluno,						
									aluno.nm_social_aluno NomeSocialAluno,
									mte.cd_turma_escola TurmaCodigo
							   FROM v_aluno_cotic aluno
							  INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
							  INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
							   LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno				   
							  WHERE mte.cd_turma_escola = @turmaCodigo
								{(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : "AND mte.cd_situacao_aluno in (1,5,6,10,13)")}								
								AND mte.dt_situacao_aluno =  (
	    													   SELECT max(mte2.dt_situacao_aluno) 
																 FROM v_historico_matricula_cotic  matr2
																INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
																WHERE matr2.cd_aluno = matr.cd_aluno
																  AND mte2.cd_turma_escola = @turmaCodigo
																  {(alunoCodigo != null ? "AND matr2.cd_aluno = @alunoCodigo" : "")}
										   
															  )
							  AND NOT EXISTS( 
	  										  SELECT 1 
												FROM v_matricula_cotic matr3
											   INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
											   WHERE mte.cd_matricula = mte3.cd_matricula
												 AND mte.cd_turma_escola = @turmaCodigo 
												 {(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : "")}
												 AND mte.cd_situacao_aluno in (1,5,6,10,13)
											 )";

			var parametros = new { turmaCodigo, alunoCodigo };

			using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
			{
				return await conexao.QueryAsync<AlunoReduzidoDto>(query, parametros);
			}
		}

		public async Task<IEnumerable<AlunoRetornoDto>> ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long turmaCodigo, long? alunoCodigo, int anoLetivo)
		{
			var query = @$"SELECT distinct aluno.cd_aluno AlunoCodigo,
					   aluno.nm_aluno NomeAluno,
					   aluno.nm_social_aluno NomeSocialAluno,
					   mte.nr_chamada_aluno NumeroAlunoChamada,
                       mte.cd_turma_escola TurmaCodigo,
                       aluno.dt_nascimento_aluno DataNascimento,
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
						    LTRIM(RTRIM(nm_responsavel)) ResponsavelNome,
		                    ra.tp_pessoa_responsavel TipoResponsavel, 
		                    LTRIM(RTRIM(ra.cd_ddd_celular_responsavel)) ResponsavelDDD, 
		                    LTRIM(RTRIM(ra.nr_celular_responsavel)) ResponsavelCelular,
		                    ra.dt_atualizacao_tabela DataAtualizacaoContato
			      FROM v_aluno_cotic aluno
				 INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
				 INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
				  LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
				  LEFT JOIN responsavel_aluno ra ON aluno.cd_aluno = ra.cd_aluno and ra.dt_fim is null
				 WHERE mte.cd_turma_escola = @turmaCodigo
				   {(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : (anoLetivo == DateTime.Now.Year ? "AND mte.cd_situacao_aluno in (1,5,6,10,13)" : "AND mte.cd_situacao_aluno = 5 "))}		 
				   
		   UNION 
				 SELECT aluno.cd_aluno AlunoCodigo,
						aluno.nm_aluno NomeAluno,						
						aluno.nm_social_aluno NomeSocialAluno,						
						mte.nr_chamada_aluno NumeroAlunoChamada,
						mte.cd_turma_escola TurmaCodigo,
						aluno.dt_nascimento_aluno DataNascimento,
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
										LTRIM(RTRIM(nm_responsavel)) ResponsavelNome,
										ra.tp_pessoa_responsavel TipoResponsavel, 
										LTRIM(RTRIM(ra.cd_ddd_celular_responsavel)) ResponsavelDDD, 
										LTRIM(RTRIM(ra.nr_celular_responsavel)) ResponsavelCelular,
										ra.dt_atualizacao_tabela DataAtualizacaoContato
				   FROM v_aluno_cotic aluno
				  INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
				  INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
				   LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
				   LEFT JOIN responsavel_aluno ra ON aluno.cd_aluno = ra.cd_aluno and ra.dt_fim is null
				  WHERE mte.cd_turma_escola = @turmaCodigo					
					{(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : (anoLetivo == DateTime.Now.Year ? "AND mte.cd_situacao_aluno in (1,5,6,10,13)" : "AND mte.cd_situacao_aluno = 5 "))}	
					AND mte.dt_situacao_aluno =  (
	    										   SELECT max(mte2.dt_situacao_aluno) 
													 FROM v_historico_matricula_cotic  matr2
													INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
													WHERE matr2.cd_aluno = matr.cd_aluno
													  AND mte2.cd_turma_escola = @turmaCodigo
													  {(alunoCodigo != null ? "AND matr2.cd_aluno = @alunoCodigo" : "")}										   
												  )
				  AND NOT EXISTS( 
	  							  SELECT 1 
									FROM v_matricula_cotic matr3
								   INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
								   WHERE mte.cd_matricula = mte3.cd_matricula
									 AND mte.cd_turma_escola = @turmaCodigo									 
									 {(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : (anoLetivo == DateTime.Now.Year ? "AND mte.cd_situacao_aluno in (1,5,6,10,13)" : "AND mte.cd_situacao_aluno = 5 "))}	
								)";

			var parametros = new { turmaCodigo, alunoCodigo };

			using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
			{
				return await conexao.QueryAsync<AlunoRetornoDto>(query, parametros);
			}
		}
	}
}
