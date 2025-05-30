﻿using SME.Pedagogico.Repository.Constantes;
using SME.SR.Data.Extensions;
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
    public class AlunoRepository : IAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<Aluno>> ObterAlunosPorTurmaDataSituacaoMatriculaParaSondagem(long turmaCodigo, DateTime dataReferenciaFim, DateTime? dataReferenciaInicio = null)
        {
            var query = $@"
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
					coalesce(mte.nr_chamada_aluno,'0') NumeroAlunoChamada
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
					and ((mte.cd_situacao_aluno in (1, 6, 10, 13, 5) or (mte.cd_situacao_aluno in (1, 6, 13, 5) and CAST(mte.dt_situacao_aluno AS DATE) < @dataReferenciaFim))
					or (mte.cd_situacao_aluno not in (1, 6, 10, 13, 5)
					{(dataReferenciaInicio == null || dataReferenciaInicio == DateTime.MinValue
                    ? "and mte.dt_situacao_aluno > @dataReferenciaFim))"
                    : "and (mte.dt_situacao_aluno > @dataReferenciaFim or (mte.dt_situacao_aluno > @dataReferenciaInicio and mte.dt_situacao_aluno <= @dataReferenciaFim))))"
                    )}
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
						mte2.cd_turma_escola = @turmaCodigo
						and matr2.cd_aluno = matr.cd_aluno
						and (matr2.st_matricula in (1, 6, 10, 13, 5)
						or (matr2.st_matricula not in (1, 6, 10, 13, 5)
                    {(dataReferenciaInicio == null || dataReferenciaInicio == DateTime.MinValue
                    ? " and matr2.dt_status_matricula > @dataReferenciaFim)))"
                    : " and (matr2.dt_status_matricula > @dataReferenciaFim or (matr2.dt_status_matricula > @dataReferenciaInicio and matr2.dt_status_matricula <= @dataReferenciaFim)))))"
                    )}
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

            var parametros = new { turmaCodigo, dataReferenciaFim, dataReferenciaInicio };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryAsync<Aluno>(query, parametros);
        }

        public async Task<IEnumerable<AlunoPorTurmaRespostaDto>> ObterAlunosPorTurmaEDataMatriculaQuery(long turmaCodigo, DateTime dataReferencia)
        {
            var query = @"
					IF OBJECT_ID('tempdb..#tmpAlunosDadosAtuais') IS NOT NULL
					DROP TABLE #tmpAlunosDadosAtuais

				IF OBJECT_ID('tempdb..#tmpAlunosFrequencia') IS NOT NULL
					DROP TABLE #tmpAlunosFrequencia

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
						ISNULL(hm.dt_status_matricula, matr.dt_status_matricula) DataMatricula,
						case when mte.nr_chamada_aluno is null then '0'
						when mte.nr_chamada_aluno = 'NULL' then '0'
						else mte.nr_chamada_aluno
						end as NumeroAlunoChamada,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia,
						LTRIM(RTRIM(nm_responsavel)) NomeResponsavel,
						ra.tp_pessoa_responsavel TipoResponsavel,
						concat(LTRIM(RTRIM(ra.cd_ddd_celular_responsavel)), LTRIM(RTRIM(ra.nr_celular_responsavel))) CelularResponsavel,
						ra.dt_atualizacao_tabela DataAtualizacaoContato,
						matr.cd_matricula CodigoMatricula
					INTO #tmpAlunosDadosAtuais
					FROM v_aluno_cotic aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						LEFT JOIN responsavel_aluno ra ON aluno.cd_aluno = ra.cd_aluno and ra.dt_fim is null
						LEFT JOIN v_historico_matricula_cotic hm ON matr.cd_matricula = hm.cd_matricula and matr.an_letivo = hm.an_letivo and hm.st_matricula <> 4 -- Não considera vínculo indevido
					WHERE mte.cd_turma_escola = @CodigoTurma

					SELECT *
						INTO #tmpAlunosFrequencia
						FROM #tmpAlunosDadosAtuais
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
						case when mte.nr_chamada_aluno is null then '0'
						when mte.nr_chamada_aluno = 'NULL' then '0'
						else mte.nr_chamada_aluno
						end as NumeroAlunoChamada,
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
					and NOT (mte.nr_chamada_aluno IS NULL AND mte.dt_situacao_aluno < te.dt_inicio_turma)
					and mte.dt_situacao_aluno =
						(select max(mte2.dt_situacao_aluno) from v_historico_matricula_cotic  matr2
						INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
						where
						mte2.cd_turma_escola = @CodigoTurma
						and matr2.cd_aluno = matr.cd_aluno
					)
					AND NOT EXISTS(
						SELECT 1
							FROM #tmpAlunosDadosAtuais da
						WHERE mte.cd_matricula = da.CodigoMatricula)

				SELECT
					alunos.CodigoAluno,
					alunos.NomeAluno,
					alunos.NomeSocialAluno,
					alunos.DataNascimento,
					alunos.CodigoSituacaoMatricula,
					alunos.SituacaoMatricula,
					alunos.DataSituacao,
					case when alunos.NumeroAlunoChamada is null then '0'
						when alunos.NumeroAlunoChamada = 'NULL' then '0'
						else alunos.NumeroAlunoChamada
						end as NumeroAlunoChamada,
					alunos.PossuiDeficiencia,
					alunos.NomeResponsavel,
					alunos.TipoResponsavel,
					alunos.CelularResponsavel,
					alunos.DataAtualizacaoContato,
					alunos.CodigoMatricula,
					alunos.DataMatricula
				FROM #tmpAlunosFrequencia alunos
				INNER JOIN (SELECT CodigoAluno, CodigoMatricula, MAX(DataMatricula) DataMatricula
							FROM #tmpAlunosFrequencia
							WHERE CONVERT(date, @DataMatricula) >= CONVERT(date, DataMatricula)
							GROUP BY CodigoAluno, CodigoMatricula) A
							ON alunos.CodigoMatricula = A.CodigoMatricula
								and alunos.DataMatricula = a.DataMatricula
				ORDER BY alunos.NomeAluno";

            var parametros = new { CodigoTurma = turmaCodigo, DataMatricula = dataReferencia };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryAsync<AlunoPorTurmaRespostaDto>(query, parametros);

        }
        public async Task<int> ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(string anoTurma, string ueCodigo, int anoLetivo, long dreCodigo, DateTime dataReferenciaFim, int[] modalidades, bool consideraHistorico = false, DateTime? dataReferenciaInicio = null)
        {
            StringBuilder query = new StringBuilder();
            if (anoLetivo < DateTime.Now.Date.Year)
                consideraHistorico = true;

            var queryHistorica = consideraHistorico ? " UNION " + ObtenhaQueryAlunosPorTurmasHistorico(dreCodigo, ueCodigo) : string.Empty;

            query.AppendLine($@" WITH lista AS (
								SELECT DISTINCT mte.cd_turma_escola,
												m.cd_aluno
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
									  or (mte.cd_situacao_aluno not in (1, 6, 10, 13, 5) and CAST(aln.DataMatricula AS DATE) <= @dataFim
									  {(dataReferenciaInicio == null || dataReferenciaInicio == DateTime.MinValue
                                        ? "and mte.dt_situacao_aluno > @dataFim))"
                                        : "and (mte.dt_situacao_aluno > @dataFim or (mte.dt_situacao_aluno > @dataInicio and mte.dt_situacao_aluno <= @dataFim))))"
                                      )}
									  and aln.AnoLetivo = anoLetivo
									  AND se.sg_resumida_serie = @anoTurma 
									  AND ee.cd_etapa_ensino in (@modalidades)
									{(dreCodigo > 0 ? " AND ue.cd_unidade_administrativa_referencia = @dreCodigo" : string.Empty)}
									{(!string.IsNullOrEmpty(ueCodigo) ? " AND ue.cd_unidade_educacao = @ueCodigo" : string.Empty)}
									{queryHistorica})
									SELECT COUNT(DISTINCT cd_aluno)
									FROM lista ");


            var parametros =
                new
                {
                    dreCodigo = dreCodigo.ToString().ToDbChar(DapperConstants.CODIGODRE_LENGTH),
                    ueCodigo,
                    anoTurma = anoTurma.ToDbChar(DapperConstants.ANOTURMA_LENGTH),
                    anoLetivo,
                    dataFim = dataReferenciaFim,
                    dataInicio = dataReferenciaInicio
                };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryFirstOrDefaultAsync<int>(query.ToString().Replace("@modalidades", string.Join(',', modalidades)), parametros, commandTimeout: 600);
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
						ExpedicaoOrgaoEmissor VARCHAR(80),
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
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

        public async Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigos(long[] codigosAlunos, int? anoLetivo)
        {
            var query = @$"
					with lista_alunos as (
					select
					cd_aluno,
					nm_aluno,
					dt_nascimento_aluno,
					nm_social_aluno,
					sg_uf_nascimento_aluno,
					cd_nacionalidade_aluno,
					nr_rg_aluno,
					cd_digito_rg_aluno,
					sg_uf_rg_aluno,
					dt_emissao_rg,
					cd_municipio_nascimento,
					cd_orgao_emissor
					from aluno
					where
					cd_aluno in @codigosAlunos),
					lista_alunos_por_codigo as (
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM lista_alunos aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE 1 = 1 {(anoLetivo.HasValue ? " and matr.an_letivo = @anoLetivo " : string.Empty)}
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
						aluno.sg_uf_rg_aluno ExpedicaoUF,
						aluno.dt_emissao_rg ExpedicaoData,
						CASE
							WHEN ISNULL(nea.tp_necessidade_especial, 0) = 0 THEN 0
							ELSE 1
						END PossuiDeficiencia
						FROM lista_alunos aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno						
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN municipio mun ON aluno.cd_municipio_nascimento = mun.cd_municipio
						LEFT JOIN orgao_emissor orge ON aluno.cd_orgao_emissor = orge.cd_orgao_emissor
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						where {(anoLetivo.HasValue ? "matr.an_letivo = @anoLetivo and " : string.Empty)}
						mte.dt_situacao_aluno in
							(SELECT mte2.dt_situacao_aluno from v_historico_matricula_cotic  matr2
								INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							 WHERE matr2.cd_aluno = matr.cd_aluno
							{(anoLetivo.HasValue ? "and matr2.an_letivo = @anoLetivo" : string.Empty)}
						)
						OR EXISTS(
							SELECT 1 FROM v_matricula_cotic matr3
								INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
							WHERE mte.cd_matricula = mte3.cd_matricula and matr3.cd_aluno = aluno.cd_aluno {(anoLetivo.HasValue ? " and matr3.an_letivo = @anoLetivo " : string.Empty)}))
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
					FROM lista_alunos_por_codigo
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
            return await conexao.QueryAsync<AlunoHistoricoEscolar>(query, new { anoLetivo, codigosAlunos }, commandTimeout: 120);
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
						  FROM v_historico_matricula_cotic matr
						  INNER JOIN v_aluno_cotic aluno ON aluno.cd_aluno = matr.cd_aluno                    
						  INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						  INNER JOIN turma_escola te2 ON mte.cd_turma_escola = te2.cd_turma_escola
						      LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						  	WHERE mte.cd_turma_escola in @codigosTurma and aluno.cd_aluno in @codigosAluno
						  	and CAST(mte.dt_situacao_aluno as date) =
						  		(select CAST(max(mte2.dt_situacao_aluno) as date) from v_historico_matricula_cotic  matr2
						  		INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
						  		where
						  		mte2.cd_turma_escola in @codigosTurma
						  		and matr2.cd_aluno = matr.cd_aluno
						  	)
						  	AND NOT EXISTS(
						  		SELECT 1 FROM v_matricula_cotic matr3
						  	INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						  	INNER JOIN turma_escola te3 ON mte3.cd_turma_escola = te3.cd_turma_escola
						  	WHERE mte.cd_matricula = mte3.cd_matricula
						  		AND mte3.cd_turma_escola in @codigosTurma
						  		AND matr3.cd_aluno = matr.cd_aluno
						  		AND te3.cd_tipo_turma = te2.cd_tipo_turma)						  
						  SELECT
						  	CodigoTurma,
						  	CodigoAluno,
						  	NomeAluno,
						  	NomeSocialAluno,
						  	DataNascimento,
						  	CodigoSituacaoMatricula,
						  	SituacaoMatricula,
						  	MAX(DataSituacao) DataSituacao,
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

            var parametros = new { codigosTurma, codigosAluno };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
                return await conexao.QueryAsync<Aluno>(query, parametros, commandTimeout: 120);
        }

        public async Task<IEnumerable<Aluno>> ObterPorCodigosTurma(IEnumerable<string> codigosTurma)
        {
            string codigo = string.Join(",", codigosTurma);

            var query = @$";WITH AlunoBase AS
			(
						SELECT     mte.cd_turma_escola       codigoturma,
									aluno.cd_aluno            codigoaluno,
									aluno.nm_aluno            nomealuno,
									aluno.dt_nascimento_aluno datanascimento,
									aluno.nm_social_aluno     nomesocialaluno,
									mte.cd_situacao_aluno     codigosituacaomatricula,
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
									END                   situacaomatricula,
									mte.dt_situacao_aluno datasituacao,
									mte.nr_chamada_aluno  numeroalunochamada,
									CASE
												WHEN isnull(nea.tp_necessidade_especial, 0) = 0 THEN 0
												ELSE 1
									END possuideficiencia
						FROM       matricula_turma_escola mte
						INNER JOIN v_matricula_cotic matr
						ON         matr.cd_matricula = mte.cd_matricula
						INNER JOIN v_aluno_cotic aluno
						ON         aluno.cd_aluno = matr.cd_aluno
						LEFT JOIN  necessidade_especial_aluno nea
						ON         nea.cd_aluno = matr.cd_aluno
						WHERE      mte.cd_turma_escola IN ({codigo})
						UNION
						SELECT     mte.cd_turma_escola       codigoturma,
									aluno.cd_aluno            codigoaluno,
									aluno.nm_aluno            nomealuno,
									aluno.dt_nascimento_aluno datanascimento,
									aluno.nm_social_aluno     nomesocialaluno,
									mte.cd_situacao_aluno     codigosituacaomatricula,
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
									END                   situacaomatricula,
									mte.dt_situacao_aluno datasituacao,
									mte.nr_chamada_aluno  numeroalunochamada,
									CASE
												WHEN isnull(nea.tp_necessidade_especial, 0) = 0 THEN 0
												ELSE 1
									END possuideficiencia
						FROM       v_historico_matricula_cotic matr
						INNER JOIN v_aluno_cotic aluno
						ON         aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte
						ON         matr.cd_matricula = mte.cd_matricula
						LEFT JOIN  necessidade_especial_aluno nea
						ON         nea.cd_aluno = matr.cd_aluno
						WHERE      mte.cd_turma_escola IN ({codigo})
						AND        mte.dt_situacao_aluno =
									(
												SELECT     max(mte2.dt_situacao_aluno)
												FROM       v_historico_matricula_cotic matr2
												INNER JOIN historico_matricula_turma_escola mte2
												ON         matr2.cd_matricula = mte2.cd_matricula
												WHERE      mte2.cd_turma_escola IN ({codigo})
												AND        matr2.cd_aluno = matr.cd_aluno)
						AND        NOT EXISTS
									(
												SELECT     1
												FROM       v_matricula_cotic matr3
												INNER JOIN matricula_turma_escola mte3
												ON         matr3.cd_matricula = mte3.cd_matricula
												WHERE      mte.cd_matricula = mte3.cd_matricula
												AND        mte3.cd_turma_escola IN ({codigo})
									)
				)

						SELECT   codigoturma,
								codigoaluno,
								nomealuno,
								nomesocialaluno,
								datanascimento,
								codigosituacaomatricula,
								situacaomatricula,
								Max(datasituacao) DataSituacao,
								numeroalunochamada,
								possuideficiencia
						FROM     AlunoBase
						GROUP BY codigoturma,
								codigoaluno,
								nomealuno,
								nomesocialaluno,
								datanascimento,
								codigosituacaomatricula,
								situacaomatricula,
								numeroalunochamada,
								possuideficiencia
						ORDER BY codigosituacaomatricula";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            {
                return await conexao.QueryAsync<Aluno>(query);
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
						ExpedicaoOrgaoEmissor VARCHAR(80),
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
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
						ExpedicaoOrgaoEmissor VARCHAR(80),
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
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
							WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
							WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
						orge.nm_orgao_emissor ExpedicaoOrgaoEmissor,
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

        public async Task<AlunoNomeDto> ObterNomeAlunoPorCodigo(string codigo)
        {
            var query = @"select vac.cd_aluno as Codigo, vac.nm_aluno as Nome, vac.nm_social_aluno as NomeSocial
                          from v_aluno_cotic vac  
                          where vac.cd_aluno = @codigo";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryFirstOrDefaultAsync<AlunoNomeDto>(query, new { codigo });
        }

        public async Task<IEnumerable<AlunoNomeDto>> ObterNomesAlunosPorCodigos(string[] codigos)
        {
            var query = @"select vac.cd_aluno as Codigo, vac.nm_aluno as Nome, vac.nm_social_aluno as NomeSocial
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

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoReduzidoDto>(query, parametros);
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
								WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
								WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
				   {(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : (anoLetivo == DateTime.Now.Year ? "AND mte.cd_situacao_aluno in (1,5,6,10,13)" : "AND mte.cd_situacao_aluno in (5, 10) "))}				   
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
											WHEN mte.cd_situacao_aluno = 16 THEN 'Transferido SED'
											WHEN mte.cd_situacao_aluno = 17 THEN 'Dispensado Ed. Física'
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
					{(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : (anoLetivo == DateTime.Now.Year ? "AND mte.cd_situacao_aluno in (1,5,6,10,13)" : "AND mte.cd_situacao_aluno in (5, 10) "))}	
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
									 {(alunoCodigo != null ? "AND aluno.cd_aluno = @alunoCodigo" : (anoLetivo == DateTime.Now.Year ? "AND mte.cd_situacao_aluno in (1,5,6,10,13)" : "AND mte.cd_situacao_aluno in (5, 10) "))}	
								)";

            var parametros = new { turmaCodigo, alunoCodigo };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoRetornoDto>(query, parametros);
        }

        public async Task<IEnumerable<DadosAlunosEscolaDto>> ObterDadosAlunosEscola(string ueCodigo, string dreCodigo, int anoLetivo, string[] codigosAlunos)
        {
            var sql = @" with matriculas as (
						select distinct CodigoAluno, isnull(NomeSocialAluno, NomeAluno) NomeAluno, DataNascimento, NomeSocialAluno, CodigoSituacaoMatricula, SituacaoMatricula, te.cd_escola AS CodigoEscola,
						case when NumeroAlunoChamada is null then '0'
						when NumeroAlunoChamada = 'NULL' then '0'
						else NumeroAlunoChamada
						end as NumeroAlunoChamada,
						DataSituacao, DataMatricula, PossuiDeficiencia, NomeResponsavel, TipoResponsavel, CelularResponsavel,
						DataAtualizacaoContato, CodigoTurma, CodigoMatricula, AnoLetivo, row_number() over (partition by CodigoMatricula, CodigoTurma order by DataSituacao desc) as Sequencia
						from alunos_matriculas_norm nm
						inner join turma_escola te on te.cd_turma_escola = nm.CodigoTurma
						inner join v_cadastro_unidade_educacao ue ON te.cd_escola = ue.cd_unidade_educacao
						where 1=1 ";

            if (!string.IsNullOrEmpty(ueCodigo) && ueCodigo != "-99")
                sql += @" AND ue.cd_unidade_educacao = @ueCodigo ";

            if (!string.IsNullOrEmpty(dreCodigo) && dreCodigo != "-99")
                sql += @" AND ue.cd_unidade_administrativa_referencia = @dreCodigo ";

            if (codigosAlunos != null && codigosAlunos.Any())
                sql += $" and CodigoAluno in @codigosAlunos ";

            sql += @" and te.an_letivo = @anoLetivo )
								select *
								from matriculas
								where sequencia in (1,2)
								and not exists (select 1
													from alunos_matriculas_norm
												where CodigoMatricula = matriculas.CodigoMatricula and
													  CodigoTurma = matriculas.CodigoTurma and
													  CodigoSituacaoMatricula = @codigoSituacaoVinculoIndevido)";

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryAsync<DadosAlunosEscolaDto>(sql,
                new
                {
                    ueCodigo,
                    dreCodigo,
                    anoLetivo,
                    codigosAlunos,
                    codigoSituacaoVinculoIndevido = (int)SituacaoMatriculaAluno.VinculoIndevido
                });
        }

        private string ObtenhaQueryAlunosPorTurmasHistorico(long dreCodigo, string ueCodigo)
        {
            return $@" SELECT
					mte.cd_turma_escola,
					matr.cd_aluno
				FROM
					v_aluno_cotic aluno
				INNER JOIN v_historico_matricula_cotic matr ON
					aluno.cd_aluno = matr.cd_aluno
				INNER JOIN historico_matricula_turma_escola mte ON
					matr.cd_matricula = mte.cd_matricula
				LEFT JOIN necessidade_especial_aluno nea ON
					nea.cd_aluno = matr.cd_aluno
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
					  mte.cd_situacao_aluno in (5, 10) AND
					  se.sg_resumida_serie = @anoTurma AND
					  ee.cd_etapa_ensino in (@modalidades)
				      AND mte.nr_chamada_aluno <> '0'
					  AND mte.nr_chamada_aluno is not null
					  {(dreCodigo > 0 ? " AND ue.cd_unidade_administrativa_referencia = @dreCodigo" : string.Empty)}
					  {(!string.IsNullOrEmpty(ueCodigo) ? " AND ue.cd_unidade_educacao = @ueCodigo" : string.Empty)}";
        }

        public async Task<NecessidadeEspecialAlunoDto> ObterNecessidadesEspeciaisPorAluno(long codigoAluno)
        {
            var query = @"SELECT 
	            a.cd_aluno as codigoAluno,
	            nea.tp_necessidade_especial as tipoNecessidadeEspecial,
	            tne.dc_necessidade_especial as descricaoNecessidadeEspecial,
	            ra.cd_tipo_recurso as tipoRecurso,
	            tra.dc_tipo_recurso as descricaoRecurso
              FROM aluno a
             inner join necessidade_especial_aluno nea ON a.cd_aluno = nea.cd_aluno and nea.dt_fim is null
             inner join tipo_necessidade_especial tne on tne.tp_necessidade_especial = nea.tp_necessidade_especial 
             left join recurso_aluno ra on ra.cd_aluno = a.cd_aluno and ra.dt_fim is null
             left join tipo_recurso_aluno tra on tra.cd_tipo_recurso = ra.cd_tipo_recurso ";

            if (codigoAluno > 0)
                query += "where a.cd_aluno = @codigoAluno";

            using var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conn.QueryFirstOrDefaultAsync<NecessidadeEspecialAlunoDto>(query, new { codigoAluno });
        }

        public async Task<(DateTime dataMatricula, DateTime dataSituacao)> ObterDatasMatriculaAlunoNaTurma(int codigoAluno, int codigoTurma)
        {
            using var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conn.QueryFirstOrDefaultAsync<(DateTime, DateTime)>(AlunoConsultas.DatasMatriculaAlunoNaTurma, new { codigoAluno, codigoTurma });
        }

        public async Task<IEnumerable<AlunoTurma>> ObterAlunosMatriculasPorTurmas(int[] codigosTurmas)
        {
            using var conn = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conn.QueryAsync<AlunoTurma>(AlunoConsultas.AlunosMatriculasPorTurmas, new { codigosTurmas });
        }

        public async Task<IEnumerable<TotalAlunosAnoTurmaDto>> ObterTotalAlunosAtivosPorPeriodoEAnoTurma(int anoLetivo, int[] modalidades, DateTime dataInicio, DateTime dataFim, string ueId, string dreId)
        {
            var parametros = new
            {
                anoLetivo,
                dataInicio,
                dataFim,
                codigoDre = dreId.ToDbChar(DapperConstants.CODIGODRE_LENGTH),
                codigoUe = ueId.ToDbChar(DapperConstants.CODIGOUE_EOL_LENGTH),
            };
            var query = AlunoConsultas.TotalDeAlunosAtivosPorPeriodo(dreId, ueId, dataInicio);

            using var con = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await con.QueryAsync<TotalAlunosAnoTurmaDto>(query.ToString().Replace("@modalidades", string.Join(", ", modalidades)), parametros, commandTimeout: 6000);
        }

        public async Task<int> ObterTotalAlunosAtivosPorTurmaEPeriodo(string codigoTurma, DateTime dataReferenciaInicio, DateTime dataReferenciaFim)
        {
            var totalAlunos = 0;
            var query = AlunoConsultas.AlunosAtivosPorTurmaEPeriodo;

            var parametros = new
            {
                dataReferenciaInicio = dataReferenciaInicio.Date,
                dataReferenciaFim = dataReferenciaFim.Date,
                codigoTurma,
                codigosSituacoesAlunoAtivo = AlunoConsultas.CodigosSituacoesAlunoAtivo
            };

            using (var con = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                var registros = await con.QueryAsync<AlunosNaTurmaDto>(query, parametros, commandTimeout: 6000);
                if (registros.Count() >= 0)
                    totalAlunos = registros.Count();
            }

            return totalAlunos;
        }
    }
}