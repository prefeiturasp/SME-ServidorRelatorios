﻿using Dapper;
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
        public async Task<IEnumerable<Aluno>> ObterAlunosPorTurmas(IEnumerable<long> turmasCodigo)
        {
            try
            {
                var query = @"	SELECT distinct aluno.cd_aluno CodigoAluno,
					   aluno.nm_aluno NomeAluno,
					   aluno.nm_social_aluno NomeSocialAluno,
					   mte.nr_chamada_aluno NumeroAlunoChamada,
                       mte.cd_turma_escola CodigoTurma
					   FROM v_aluno_cotic aluno
						INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @turmasId and mte.cd_situacao_aluno in (1,5,6,10,13)
						UNION 
						SELECT  aluno.cd_aluno CodigoAluno,
						aluno.nm_aluno NomeAluno,						
						aluno.nm_social_aluno NomeSocialAluno,						
						mte.nr_chamada_aluno NumeroAlunoChamada,
                        mte.cd_turma_escola CodigoTurma
						FROM v_aluno_cotic aluno
						INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						WHERE mte.cd_turma_escola in @turmasId and mte.cd_situacao_aluno in (1,5,6,10,13)
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
							AND mte.cd_turma_escola  in @turmasId and mte.cd_situacao_aluno in (1,5,6,10,13))";
                var parametros = new { turmasId = turmasCodigo };

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
			                t.modalidade_codigo  ModalidadeCodigo, t.semestre, t.ano, t.ano_letivo AnoLetivo, tc.descricao Ciclo, t.etapa_eja EtapaEJA,
			                ue.id, ue.ue_id Codigo, ue.nome, ue.tipo_escola TipoEscola,		
			                dre.id, dre.dre_id Codigo, dre.abreviacao, dre.nome
			                from  turma t
			                inner join ue on ue.id = t.ue_id 
			                inner join dre on ue.dre_id = dre.id 
                            left join tipo_ciclo_ano tca on t.modalidade_codigo = tca.modalidade and t.ano = tca.ano
                            left join tipo_ciclo tc on tca.tipo_ciclo_id = tc.id
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
	                        t.ano,
                            t.etapa_eja as EtapaEJA
                        from
	                        fechamento_turma ft
                        inner join conselho_classe cc on
	                        cc.fechamento_turma_id = ft.id
                        inner join conselho_classe_aluno cca on
	                        cca.conselho_classe_id = cc.id
	                     inner join turma t 
	                     	on ft.turma_id = t.id
                        where
                            not ft.excluido 
                            and not cc.excluido 
                            and ft.periodo_escolar_id is null
	                        and cca.aluno_codigo = any(@codigoAlunos) 
	                        and cca.conselho_classe_parecer_id  = any(@codigoPareceresConclusivos)";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            var codigos = codigoAlunos.Select(a => a.ToString()).ToArray();

            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { codigoAlunos = codigos, codigoPareceresConclusivos });

        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaParecerConclusivo(long turmaCodigo, long[] codigoPareceresConclusivos)
        {
            var query = @"select distinct 
	                        t.turma_id as TurmaCodigo,
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
                        where not ft.excluido 
                           and not cc.excluido 
                           and ft.periodo_escolar_id is null
	                       and t.turma_id = @turmaCodigo
	                       and cca.conselho_classe_parecer_id = any(@codigoPareceresConclusivos)";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { turmaCodigo = turmaCodigo.ToString(), codigoPareceresConclusivos });

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

        public async Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterPorUeCicloAno(string ano, long tipoCicloId, long ueId)
        {
            var query = @"select t.turma_id as codigo, t.id, t.nome from  tipo_ciclo tc 
                        inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                        inner join turma t on tca.ano = t.ano and tca.modalidade = t.modalidade_codigo
                        inner join ue u on t.ue_id  = u.id
                        where u.id = @ueId and tc.id = @tipoCicloId and tca.ano = @ano";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<TurmaFiltradaUeCicloAnoDto>(query, new { ueId, tipoCicloId, ano });
        }
        public async Task<IEnumerable<AlunoTurmaRegularRetornoDto>> ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery(long turmaCodigo)
        {
            var query = @"declare @Ano int
                    declare @CdTipoTurma int
                    declare @CdEscola int									

                    if object_id('tempdb..#tempAlu') is not null
                        drop table #tempAlu

                    if object_id('tempdb..#tempMat') is not null
                        drop table #tempMat


                    SELECT TOP 1 @CdTipoTurma = te.cd_tipo_turma,
                                 @Ano = te.an_letivo,
                                 @CdEscola = te.cd_escola
                    FROM matricula_turma_escola mte
                             INNER JOIN turma_escola te
                                        ON mte.cd_turma_escola = te.cd_turma_escola
                    WHERE mte.cd_turma_escola = @CodigoTurma

                    IF (@CdTipoTurma IS NULL)
                        SELECT TOP 1 @CdTipoTurma = te.cd_tipo_turma,
                                     @Ano = te.an_letivo,
                                     @CdEscola = te.cd_escola
                        FROM historico_matricula_turma_escola mte
                                 INNER JOIN turma_escola te
                                            ON mte.cd_turma_escola = te.cd_turma_escola
                        WHERE mte.cd_turma_escola = @CodigoTurma


                    IF (@CdTipoTurma <> 1)
                        BEGIN
                            select distinct cd_aluno,
                                            gcc.cd_componente_curricular
                            into #tempAlu
                            from (select cd_matricula,
                                         cd_turma_escola
                                  from matricula_turma_escola
                                  where cd_turma_escola = @CodigoTurma
                                    and cd_situacao_aluno in (1, 6, 10, 13)
                                  union all
                                  select cd_matricula,
                                         cd_turma_escola
                                  from historico_matricula_turma_escola
                                  where cd_turma_escola = @CodigoTurma
                                    and cd_situacao_aluno in (1, 6, 10, 13)) mat_tur
                                     inner join v_matricula_cotic v_mat_pro on v_mat_pro.cd_matricula = mat_tur.cd_matricula
                                     inner join turma_escola_grade_programa tegp on mat_tur.cd_turma_escola = tegp.cd_turma_escola
                                     inner join escola_grade eg on eg.cd_escola_grade = tegp.cd_escola_grade and dt_fim is null
                                     inner join grade_componente_curricular gcc on eg.cd_grade = gcc.cd_grade;

                            select cd_matricula,
                                   cd_aluno,
                                   an_letivo,
                                   cd_rendimento_parecer_conclusivo
                            into #tempMat
                            from (select cd_matricula,
                                         cd_aluno,
                                         an_letivo,
                                         cd_rendimento_parecer_conclusivo,
                                         row_number() over (partition by cd_aluno,an_letivo order by dt_status_matricula desc ) ordem
                                  from (
                                           select cd_matricula,
                                                  dt_status_matricula,
                                                  cd_rendimento_parecer_conclusivo,
                                                  an_letivo,
                                                  cd_aluno
                                           from v_matricula_cotic
                                           where cd_aluno in (SELECT cd_aluno from #tempAlu)
                                             and cd_serie_ensino is not null
                                             and an_letivo >= @Ano - 1
                                             and ((an_letivo = @Ano - 1 and cd_rendimento_parecer_conclusivo is not null) or
                                                  an_letivo = @Ano)
                                           union all
                                           select cd_matricula,
                                                  dt_status_matricula,
                                                  cd_rendimento_parecer_conclusivo,
                                                  an_letivo,
                                                  cd_aluno
                                           from v_historico_matricula_cotic
                                           where cd_aluno in (SELECT cd_aluno from #tempAlu)
                                             and cd_serie_ensino is not null
                                             and an_letivo >= @Ano - 1
                                             and ((an_letivo = @Ano - 1 and cd_rendimento_parecer_conclusivo is not null) or
                                                  an_letivo = @Ano)
                                       ) joinMat) mat
                            where ordem = 1;


                            select distinct alu.cd_aluno                                          as AlunoCodigo,                                            
                                            te.dc_turma_escola                                    as TurmaNome                                            
                            from (
                                     select *, row_number() over (partition by cd_matricula order by dt_situacao_aluno desc ) as ordem
                                     from (
                                              select cd_matricula,
                                                     cd_turma_escola,
                                                     dt_situacao_aluno,
                                                     cd_situacao_aluno,
                                                     nr_chamada_aluno
                                              from matricula_turma_escola
                                              where cd_matricula in (select cd_matricula
                                                                     from #tempMat
                                                                     where an_letivo = @Ano)
                                                and cd_situacao_aluno in (1, 6, 10, 13)
                                              union all
                                              select cd_matricula,
                                                     cd_turma_escola,
                                                     dt_situacao_aluno,
                                                     cd_situacao_aluno,
                                                     nr_chamada_aluno
                                              from historico_matricula_turma_escola
                                              where cd_matricula in (select cd_matricula
                                                                     from #tempMat
                                                                     where an_letivo = @Ano)
                                                and cd_situacao_aluno in (1, 6, 10, 13)
                                          ) mte
                                 ) mte
                                     inner join turma_escola te on mte.cd_turma_escola = te.cd_turma_escola
                                     inner join #tempMat mat on mat.cd_matricula = mte.cd_matricula and te.an_letivo = mat.an_letivo
                                     inner join v_aluno_cotic alu on mat.cd_aluno = alu.cd_aluno
                                     left join #tempMat conclu
                                               on conclu.cd_aluno = alu.cd_aluno and conclu.cd_rendimento_parecer_conclusivo is not null
                                     left join necessidade_especial_aluno nea on alu.cd_aluno = nea.cd_aluno and nea.dt_fim is null
                                     left join #tempAlu tA on alu.cd_aluno = tA.cd_aluno
                            where mte.ordem = 1;
                        END
                    ELSE
                        BEGIN
                            SELECT distinct cd_aluno            as AlunoCodigo,
                                            dc_turma_escola     as TurmaNome                                            
                            from (
                                     select mte.cd_turma_escola,
                                            v_mat.cd_aluno,
                                            valu.nm_aluno,
                                            valu.dt_nascimento_aluno,
                                            valu.nm_social_aluno,
                                            te.dc_turma_escola,
                                            te.an_letivo,
                                            mte.cd_situacao_aluno,
                                            CASE
                                                WHEN mte.cd_situacao_aluno = 1 THEN 'Ativo'
                                                WHEN mte.cd_situacao_aluno = 6 THEN 'Pendente de Rematricula'
                                                WHEN mte.cd_situacao_aluno = 10 THEN 'Rematriculado'
                                                WHEN mte.cd_situacao_aluno = 13 THEN 'Sem continuidade'
                                                END as                                            SituacaoMatricula,
                                            mte.dt_situacao_aluno,
                                            mte.nr_chamada_aluno,
                                            gcc.cd_componente_curricular,
                                            IIF(ISNULL(nea.tp_necessidade_especial, 0) = 0, 0, 1) PossuiDeficiencia
                                     from (
                                              select *,
                                                     row_number() over (partition by cd_matricula order by dt_situacao_aluno desc) ordem
                                              from (
                                                       select cd_matricula,
                                                              cd_turma_escola,
                                                              dt_situacao_aluno,
                                                              cd_situacao_aluno,
                                                              tp_origem_matricula,
                                                              nr_chamada_aluno
                                                       from matricula_turma_escola
                                                       where cd_turma_escola = @CodigoTurma
                                                   ) mte
                                          ) mte
                                              inner join v_matricula_cotic v_mat on v_mat.cd_matricula = mte.cd_matricula
                                              inner join turma_escola te on te.cd_turma_escola = mte.cd_turma_escola
                                              inner join serie_turma_grade stg
                                                         on te.cd_turma_escola = stg.cd_turma_escola and stg.dt_fim is null
                                              inner join escola_grade eg on stg.cd_escola_grade = eg.cd_escola_grade
                                              inner join grade g on eg.cd_grade = g.cd_grade
                                              inner join grade_componente_curricular gcc
                                                         on g.cd_grade = gcc.cd_grade and g.dt_fim_validade is null
                                              inner join v_aluno_cotic valu on valu.cd_aluno = v_mat.cd_aluno
                                              left join necessidade_especial_aluno nea
                                                        on valu.cd_aluno = nea.cd_aluno and nea.dt_fim is null

                                     where mte.ordem = 1
                                       and mte.cd_situacao_aluno in (1, 6, 10, 13)
                                       and exists(select 1
                                                  from atribuicao_aula aa
                                                  where aa.cd_grade = g.cd_grade
                                                    and aa.an_atribuicao = te.an_letivo
                                                    and aa.cd_serie_grade = stg.cd_serie_grade
                                                    and aa.dt_cancelamento is null
                                                    and aa.dt_disponibilizacao_aulas is null)
                                 ) treg
                        END";


            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

            return await conexao.QueryAsync<AlunoTurmaRegularRetornoDto>(query, new { codigoTurma = turmaCodigo });


        }

        public async Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterTurmasPorUeAnosModalidadeESemestre(string[] uesCodigos, string[] anos, int modalidade, int? semestre)
        {
            try
            {
                var query = new StringBuilder(@"select t.turma_id as codigo, t.id, u.id as ueId, t.nome from  tipo_ciclo tc 
                        inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                        inner join turma t on tca.ano = t.ano and tca.modalidade = t.modalidade_codigo
                        inner join ue u on t.ue_id  = u.id
                        where 1=1 ");

                if (anos != null && anos.Length > 0)
                    query.AppendLine("and tca.ano = ANY(@anos) ");

                if (uesCodigos != null && uesCodigos.Length > 0)
                    query.AppendLine("and u.ue_id = ANY(@uesCodigos) ");

                if (modalidade > 0)
                    query.AppendLine("and t.modalidade_codigo = @modalidade ");

                if (semestre.HasValue)
                    query.AppendLine("and t.semestre = @semestre ");

                //query.AppendLine("order by tca.ano ");

                using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

                return await conexao.QueryAsync<TurmaFiltradaUeCicloAnoDto>(query.ToString(), new { uesCodigos, anos, modalidade, semestre = semestre ?? 0 });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Aluno>> ObterDadosAlunosPorTurmaNotasConceitos(string codigoTurma)
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
							WHEN mte.cd_situacao_aluno = 5 THEN 'Concluído'
							WHEN mte.cd_situacao_aluno = 6 THEN 'Pendente de Rematrícula'
							WHEN mte.cd_situacao_aluno = 10 THEN 'Rematriculado'
							WHEN mte.cd_situacao_aluno = 13 THEN 'Sem continuidade'
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
							WHEN mte.cd_situacao_aluno = 5 THEN 'Concluído'
							WHEN mte.cd_situacao_aluno = 6 THEN 'Pendente de Rematrícula'
							WHEN mte.cd_situacao_aluno = 10 THEN 'Rematriculado'
							WHEN mte.cd_situacao_aluno = 13 THEN 'Sem continuidade'
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
            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<Aluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosSemParecerConclusivo(long[] codigoAlunos)
        {
            var query = @"select distinct 
	                        t.turma_id as TurmaCodigo,
                            t.modalidade_codigo Modalidade,
	                        cca.aluno_codigo as AlunoCodigo,
	                        t.ano,
                            t.etapa_eja as EtapaEJA,
                            tc.descricao as Ciclo
                        from
	                        fechamento_turma ft
                        inner join conselho_classe cc on
	                        cc.fechamento_turma_id = ft.id
                        inner join conselho_classe_aluno cca on
	                        cca.conselho_classe_id = cc.id
	                     inner join turma t 
	                     	on ft.turma_id = t.id
                         left join tipo_ciclo_ano tca on t.modalidade_codigo = tca.modalidade and t.ano = tca.ano
                         left join tipo_ciclo tc on tca.tipo_ciclo_id = tc.id
                        where not ft.excluido 
                            and not cc.excluido 
                            and ft.periodo_escolar_id is not null
	                        and cca.aluno_codigo = any(@codigoAlunos) 
	                        and cca.conselho_classe_parecer_id is null
                            and not t.historica  
                            and not exists (select 1
                            from
	                            fechamento_turma ft2
                            inner join conselho_classe cc2 on
	                            cc2.fechamento_turma_id = ft2.id
                            inner join conselho_classe_aluno cca2 on
	                            cca2.conselho_classe_id = cc2.id
	                         where not ft2.excluido 
                                and not cc2.excluido 
                                and ft2.turma_id = ft.turma_id 
	                    	    and	cca2.aluno_codigo = cca.aluno_codigo 
	                    	    and ft2.periodo_escolar_id is null
                                and cca2.conselho_classe_parecer_id is not null)        ";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            var codigos = codigoAlunos.Select(a => a.ToString()).ToArray();

            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { codigoAlunos = codigos });
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaSemParecerConclusivo(long turmaCodigo)
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
                        where not ft.excluido 
                            and not cc.excluido 
                            and ft.periodo_escolar_id is not null
	                        and t.turma_id = @turmaCodigo
	                        and cca.conselho_classe_parecer_id is null
                            and not t.historica   
                            and not exists (select 1
                            from
	                            fechamento_turma ft2
                            inner join conselho_classe cc2 on
	                            cc2.fechamento_turma_id = ft2.id
                            inner join conselho_classe_aluno cca2 on
	                            cca2.conselho_classe_id = cc2.id
	                         where not ft2.excluido 
                                and not cc2.excluido 
                                and ft2.turma_id = ft.turma_id 
	                    	    and	cca2.aluno_codigo = cca.aluno_codigo 
	                    	    and ft2.periodo_escolar_id is null)";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { turmaCodigo = turmaCodigo.ToString() });
        }

        public async Task<IEnumerable<long>> ObterTurmasCodigoPorUeAnoSondagemAsync(string ano, string ueCodigo, int anoLetivo, long dreCodigo)
        {
            try
            {

                var query = new StringBuilder(@"
                     SELECT distinct turma.cd_turma_escola  codigoTurma 
                     FROM turma_escola Turma  
                     INNER JOIN serie_turma_escola serie_turma
					 ON serie_turma.cd_turma_escola = turma.cd_turma_escola
					 INNER JOIN serie_ensino serie_ensino
					 ON serie_turma.cd_serie_ensino = serie_ensino.cd_serie_ensino
					 INNER JOIN etapa_ensino etapa_ensino
					 ON etapa_ensino.cd_etapa_ensino = serie_ensino.cd_etapa_ensino
                     AND etapa_ensino.cd_modalidade_ensino = 1
					 AND etapa_ensino.cd_etapa_ensino = 5 AND etapa_ensino.dt_cancelamento is null
                       INNER JOIN v_cadastro_unidade_educacao cue
					 on cue.cd_unidade_educacao = turma.cd_escola
					 WHERE an_letivo = @anoLetivo and Turma.st_turma_escola <> 'E'
                     AND left(dc_turma_escola, 1) = @ano AND Turma.cd_tipo_turma = 1");


                if (!string.IsNullOrEmpty(ueCodigo))
                    query.AppendLine("AND turma.cd_escola = @ueCodigo");

                if (dreCodigo > 0)
                    query.AppendLine("AND cue.cd_unidade_administrativa_referencia = @dreCodigo");


                using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);

                return await conexao.QueryAsync<long>(query.ToString(), new { ano, ueCodigo, anoLetivo, dreCodigo });

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<TurmaResumoDto> ObterTurmaResumoComDreUePorId(long turmaId)
        {
            var query = @"select t.id, t.nome, t.ano_letivo as AnoLetivo, t.modalidade_codigo as Modalidade
	                            , ue.id, ue.ue_id as CodigoUe, ue.nome, dre.id, dre.abreviacao, dre.nome
                              from turma t
                             inner join ue on ue.id = t.ue_id
                             inner join dre on dre.id = ue.dre_id
                             where t.id = @turmaId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return (await conexao.QueryAsync<TurmaResumoDto, UeDto, DreDto, TurmaResumoDto>(query, (turmaDto, ueDto, dreDto) =>
                {
                    ueDto.Dre = dreDto;
                    turmaDto.Ue = ueDto;

                    return turmaDto;
                },
                new { turmaId })).First();
            }
        }
    }
}
