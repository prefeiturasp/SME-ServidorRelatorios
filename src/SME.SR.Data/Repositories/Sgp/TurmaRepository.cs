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
        public async Task<IEnumerable<AlunoDaTurmaDto>> ObterAlunosPorTurmas(IEnumerable<long> turmasCodigo)
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
            return await conexao.QueryAsync<AlunoDaTurmaDto>(query, parametros);
        }

        public async Task<IEnumerable<AlunoDaTurmaDto>> ObterAlunosPorTurmasAnosAnteriores(IEnumerable<long> turmasCodigo)
        {
            var query = @"	
						    SELECT  
                                    aluno.cd_aluno CodigoAluno,
						            aluno.nm_aluno NomeAluno,						
						            aluno.nm_social_aluno NomeSocialAluno,						
						            mte.nr_chamada_aluno NumeroAlunoChamada,
                                    mte.cd_turma_escola CodigoTurma
						        FROM v_aluno_cotic aluno
						        INNER JOIN v_historico_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
						        INNER JOIN historico_matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
						        LEFT JOIN necessidade_especial_aluno nea ON nea.cd_aluno = matr.cd_aluno
						        WHERE mte.cd_turma_escola in @turmasId                                 
                                AND mte.cd_situacao_aluno in (1,5,6,10,13)                                 
						        AND mte.dt_situacao_aluno =                    
							                                ( SELECT MAX(mte2.dt_situacao_aluno)
                                                                FROM v_historico_matricula_cotic  matr2
							                                    INNER JOIN historico_matricula_turma_escola mte2 ON matr2.cd_matricula = mte2.cd_matricula
							                                    WHERE mte2.cd_turma_escola in @turmasId
							                                    AND matr2.cd_aluno = matr.cd_aluno
						                                    )
						        AND NOT EXISTS(
							                    SELECT 1 
                                                    FROM v_matricula_cotic matr3
						                        INNER JOIN matricula_turma_escola mte3 ON matr3.cd_matricula = mte3.cd_matricula
						                        WHERE mte.cd_matricula = mte3.cd_matricula
							                        AND mte.cd_turma_escola  in @turmasId 
                                                    AND mte.cd_situacao_aluno in (1,5,6,10,13)
                                                )";
            var parametros = new { turmasId = turmasCodigo };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<AlunoDaTurmaDto>(query, parametros, commandTimeout: 300);

        }
        public async Task<IEnumerable<AlunoSituacaoDto>> ObterDadosAlunosSituacao(string turmaCodigo)
        {
            var query = TurmaConsultas.DadosAlunosSituacao;

            using (var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
            {
                return await conexao.QueryAsync<AlunoSituacaoDto>(query, new { turmaCodigo }, commandTimeout: 120);
            }
        }

        public async Task<DreUe> ObterDreUe(string codigoTurma)
        {
            var query = TurmaConsultas.DadosCompletosDreUe;
            var parametros = new { CodigoTurma = codigoTurma };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryFirstOrDefaultAsync<DreUe>(query, parametros);
        }

        public async Task<Turma> ObterComDreUePorCodigo(string codigoTurma)
        {
            var query = @"select t.turma_id Codigo, t.nome, 
			                t.modalidade_codigo  ModalidadeCodigo, t.semestre, t.ano, t.ano_letivo AnoLetivo, tc.descricao Ciclo, t.etapa_eja EtapaEJA, t.tipo_turma TipoTurma,
			                ue.id, ue.ue_id Codigo, ue.nome, ue.tipo_escola TipoEscola,		
			                dre.id, dre.dre_id Codigo, dre.abreviacao, dre.nome
			                from  turma t
			                inner join ue on ue.id = t.ue_id 
			                inner join dre on ue.dre_id = dre.id 
                            left join tipo_ciclo_ano tca on t.modalidade_codigo = tca.modalidade and t.ano = tca.ano
                            left join tipo_ciclo tc on tca.tipo_ciclo_id = tc.id
			                where t.turma_id = @codigoTurma";

            var parametros = new { CodigoTurma = codigoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
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

        public async Task<Turma> ObterComDreUePorId(long turmaId)
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
			                where t.Id = @turmaId";

            var parametros = new { turmaId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
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

        public async Task<IEnumerable<Turma>> ObterPorAbrangenciaFiltros(string codigoUe, Modalidade modalidade, int anoLetivo, string login, Guid perfil, bool consideraHistorico, int semestre, bool? possuiFechamento = null, bool? somenteEscolarizada = null, string codigoDre = null)
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


            if (!string.IsNullOrEmpty(codigoDre))
                query.Append(@" and codigo in (select t.turma_id from turma t
                                 inner join ue on ue.id = t.ue_id
                                 inner join dre on dre.id = ue.dre_id
                                 where dre.dre_id = @codigoDre)");

            var parametros = new
            {
                CodigoDre = codigoDre,
                CodigoUe = codigoUe,
                Modalidade = (int)modalidade,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Login = login,
                Perfil = perfil,
                ConsideraHistorico = consideraHistorico
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var turmas = await conexao.QueryAsync<Turma>(query.ToString(), parametros);

                query.Clear();
                query.AppendLine("select cd_turma_escola");
                query.AppendLine("  from turma_escola");
                query.AppendLine("where cd_tipo_turma = 1 and");
                query.AppendLine("      cd_turma_escola in ('#codigosTurmas')");

                using (var conexaoEol = new SqlConnection(variaveisAmbiente.ConnectionStringEol))
                {
                    var turmasRegulares = await conexaoEol.QueryAsync<string>(
                        query.ToString().Replace("#codigosTurmas", string.Join("', '", turmas.Select(t => t.Codigo))));

                    return turmas.Where(t => turmasRegulares.Contains(t.Codigo));
                }
            }
        }

        public async Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosEParecerConclusivo(long[] codigoAlunos, long[] codigoPareceresConclusivos)
        {
            var query = @"drop table if exists tempAlunosConselhoDeClasse;
                        select distinct 
	                        cca.aluno_codigo as AlunoCodigo,
	                        cc.id as ConselhoClasseId,
	                        cca.conselho_classe_parecer_id as ParecerConclusivo
                        into temp tempAlunosConselhoDeClasse
                        from
	                        fechamento_turma ft
                        inner join conselho_classe cc on
	                        cc.fechamento_turma_id = ft.id
                        inner join conselho_classe_aluno cca on
	                        cca.conselho_classe_id = cc.id
                        inner join 
	                        turma t 
	                        on ft.turma_id = t.id
                        where not ft.excluido 
                           and not cc.excluido 
                           and ft.periodo_escolar_id is null
                           and cca.aluno_codigo = any(@codigoAlunos) 
                           and cca.conselho_classe_parecer_id = any(@codigoPareceresConclusivos);

                        -- Obter turma regular
                        drop table if exists tempAlunosTurmasRegulares;
                        select 
	                        t.turma_id as TurmaCodigo,
                            null as RegularCodigo,
	                        t.modalidade_codigo Modalidade,
	                        t1.AlunoCodigo,
	                        t.ano,
	                        t.etapa_eja as EtapaEJA,
	                        t1.ParecerConclusivo,
	                        tc.descricao Ciclo,
                            t.tipo_turma as TipoTurma
                        into temp tempAlunosTurmasRegulares
                        from 
	                        tempAlunosConselhoDeClasse t1
                        inner join 
	                        conselho_classe cc
	                        on cc.id = t1.ConselhoClasseId
                        inner join
	                        fechamento_turma ft
	                        on cc.fechamento_turma_id = ft.id
                        inner join 
	                        turma t
	                        on ft.turma_id = t.id
                        inner join 
	                        tipo_ciclo_ano tca 
	                        on tca.modalidade = t.modalidade_codigo and tca.ano = t.ano
                        inner join 
	                        tipo_ciclo tc 
	                        on tc.id = tca.tipo_ciclo_id;

                        -- Obter turmas complementares
                        drop table if exists tempAlunosTurmasComplementares;
                        select 
	                        t.turma_id as TurmaCodigo,
                            tr.turma_id as RegularCodigo,
	                        t.modalidade_codigo Modalidade,
	                        t1.AlunoCodigo,
	                        t.ano,
	                        t.etapa_eja as EtapaEJA,
	                        t1.ParecerConclusivo,
	                        tc.descricao Ciclo,
                            t.tipo_turma as TipoTurma
                        into temp tempAlunosTurmasComplementares  
                        from 
	                        tempAlunosConselhoDeClasse t1
                        inner join
	                        conselho_classe_aluno cca
	                        on t1.ConselhoClasseId = cca.conselho_classe_id and t1.AlunoCodigo = cca.aluno_codigo
                        inner join 
	                        conselho_classe_aluno_turma_complementar ccat
	                        on cca.id = ccat.conselho_classe_aluno_id
                        inner join 
	                        conselho_classe cc
	                        on cc.id = t1.ConselhoClasseId
                        inner join
	                        fechamento_turma ft
	                        on cc.fechamento_turma_id = ft.id
                        inner join 
	                        turma tr
	                        on tr.id = ft.turma_id
                        inner join 
	                        turma t
	                        on t.id = ccat.turma_id
                        inner join 
	                        tipo_ciclo_ano tca 
	                        on tca.modalidade = t.modalidade_codigo and tca.ano = t.ano
                        inner join 
	                        tipo_ciclo tc 
	                        on tc.id = tca.tipo_ciclo_id;

                        select 
	                        *
                        from 
	                        (select * from tempAlunosTurmasRegulares) as regulares
                        union
	                        (select * from tempAlunosTurmasComplementares);";


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


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<Turma>(query.ToString(), new { anoLetivo, anosEscolares, modalidade });
            }

        }

        public async Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterPorUeCicloAno(int anoLetivo, string ano, long tipoCicloId, long ueId)
        {
            var query = @"select t.turma_id as codigo, t.id, t.nome from  tipo_ciclo tc 
                        inner join tipo_ciclo_ano tca on tc.id = tca.tipo_ciclo_id
                        inner join turma t on tca.ano = t.ano and tca.modalidade = t.modalidade_codigo
                        inner join ue u on t.ue_id  = u.id
                        where u.id = @ueId and tc.id = @tipoCicloId and t.ano_letivo = @anoLetivo and tca.ano = @ano and t.tipo_turma = @tipoTurmaRegular";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<TurmaFiltradaUeCicloAnoDto>(query, new { ueId, tipoCicloId, anoLetivo, ano, tipoTurmaRegular = TipoTurma.Regular });
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

                using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

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
            var query = @";with tempTurmaRegularConselhoAluno as
                        (select distinct 
                            t.turma_id as TurmaCodigo,
                            null as TurmaRegularCodigo,
                            t.modalidade_codigo Modalidade,
                            cca.aluno_codigo as AlunoCodigo,
                            t.ano,
                            t.etapa_eja as EtapaEJA,
                            tc.descricao as Ciclo,
                            t.tipo_turma as TipoTurma,
                            cca.id as ConselhoClasseAlunoId
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
                                and cca2.conselho_classe_parecer_id is not null)
                        ), tempConselhoAlunos as 
                        -- Obter turmas complementares
                        (select 
                            distinct 
                            ConselhoClasseAlunoId
                        from 
                            tempTurmaRegularConselhoAluno
                        ), tempTurmaComplementarConselhoAluno as
                        (select distinct 
                            t.turma_id as TurmaCodigo,
                            tr.turma_id as TurmaRegularCodigo,
                            t.modalidade_codigo Modalidade,
                            cca.aluno_codigo as AlunoCodigo,
                            t.ano,
                            t.etapa_eja as EtapaEJA,
                            tc.descricao as Ciclo,
                            t.tipo_turma as TipoTurma
                        from 
                            tempConselhoAlunos t1
                        inner join
                            conselho_classe_aluno cca 
                            on t1.ConselhoClasseAlunoId = cca.id 
                        inner join 
                            conselho_classe_aluno_turma_complementar ccatc 
                            on cca.id = ccatc.conselho_classe_aluno_id 
                         inner join 
                            conselho_classe cc
                            on cc.id = cca.conselho_classe_id
                        inner join
                            fechamento_turma ft
                            on cc.fechamento_turma_id = ft.id
                        inner join 
                            turma tr
                            on tr.id = ft.turma_id
                        inner join 
                            turma t
                            on ccatc.turma_id = t.id
                        left join 
                            tipo_ciclo_ano tca 
                            on t.modalidade_codigo = tca.modalidade and t.ano = tca.ano
                        left join 
                            tipo_ciclo tc 
                            on tca.tipo_ciclo_id = tc.id)

                        -- Resultado
                        select 
                            *
                        from 
                            (select TurmaCodigo,TurmaRegularCodigo,Modalidade,AlunoCodigo,ano,EtapaEJA,Ciclo,TipoTurma from tempTurmaRegularConselhoAluno) as Regulares
                        union
                            (select * from tempTurmaComplementarConselhoAluno)";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { codigoAlunos = codigoAlunos.Select(a => a.ToString()).ToArray() });
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
	                    	    and ft2.periodo_escolar_id is null
                                and cca2.conselho_classe_parecer_id is not null)";


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<AlunosTurmasCodigosDto>(query, new { turmaCodigo = turmaCodigo.ToString() });
        }

        public async Task<IEnumerable<long>> ObterTurmasCodigoPorUeAnoSondagemAsync(string ano, string ueCodigo, int anoLetivo, long dreCodigo)
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

        public async Task<TurmaResumoDto> ObterTurmaResumoComDreUePorId(long turmaId)
        {
            var query = @"select t.id, t.turma_id as Codigo, t.nome, t.ano_letivo as AnoLetivo, t.modalidade_codigo as Modalidade
                                , ue.id, ue.ue_id as CodigoUe, ue.nome, ue.tipo_escola as TipoEscola
                                , dre.id, dre.abreviacao, dre.nome
                              from turma t
                             inner join ue on ue.id = t.ue_id
                             inner join dre on dre.id = ue.dre_id
                             where t.id = @turmaId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var turma = (await conexao.QueryAsync<TurmaResumoDto, UeDto, DreDto, TurmaResumoDto>(query, (turmaDto, ueDto, dreDto) =>
                {
                    ueDto.Dre = dreDto;
                    turmaDto.Ue = ueDto;

                    return turmaDto;
                },
                new { turmaId }));

                return turma.First();
            }
        }
        public async Task<IEnumerable<TurmaResumoDto>> ObterTurmasResumoPorCodigos(string[] turmaCodigos)
        {
            var query = @"select t.id, t.nome, t.ano_letivo as AnoLetivo, t.modalidade_codigo as Modalidade, t.turma_id as codigo                                
                              from turma t                             
                             where t.turma_id = Any(@turmaCodigos)";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<TurmaResumoDto>(query.ToString(), new { turmaCodigos });
        }
        public async Task<IEnumerable<Turma>> ObterTurmasPorIds(long[] ids)
        {
            var query = @"select t.id 
                            , t.turma_id as Codigo
                            , t.nome
                            , t.modalidade_codigo  ModalidadeCodigo
                            , t.semestre
                            , t.ano
                            , t.ano_letivo AnoLetivo
                        from turma t
                       where t.id = ANY(@ids)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<Turma>(query, new { ids });
            }
        }

        public async Task<IEnumerable<Turma>> ObterTurmasPorUeEAnoLetivo(string ueCodigo, long anoLetivo)
        {
            var query = @"select t.id 
   		                        , t.turma_id as Codigo
                                , t.nome
                                , t.modalidade_codigo  ModalidadeCodigo
                                , t.semestre
                                , t.ano
                                , t.ano_letivo AnoLetivo
                            from turma t
                           inner join ue on ue.id = t.ue_id 
                           where ue.ue_id = @ueCodigo
                             and t.ano_letivo = @anoLetivo";
            try
            {
                using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
                {
                    return await conexao.QueryAsync<Turma>(query, new { ueCodigo, anoLetivo });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<Turma> ObterPorId(long id)
        {
            var query = @"select t.turma_id Codigo, t.nome
			                    , t.modalidade_codigo  ModalidadeCodigo, t.semestre
                                , t.ano, t.ano_letivo AnoLetivo
			                from turma t
			                where t.id = @id";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<Turma>(query, new { id });
            }
        }

        public async Task<Turma> ObterPorCodigo(string turmaCodigo)
        {
            var query = @"select t.turma_id Codigo, t.nome
			                    , t.modalidade_codigo  ModalidadeCodigo, t.semestre
                                , t.ano, t.ano_letivo AnoLetivo, t.tipo_turma TipoTurma
			                from turma t
			                where t.turma_id = @turmaCodigo";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<Turma>(query, new { turmaCodigo });
            }
        }

        public async Task<IEnumerable<Turma>> ObterTurmasDetalhePorCodigos(long[] turmaCodigos)
        {
            var query = @"  SELECT DISTINCT	
                                    tur.cd_escola AS ueCodigo,
				                    tur.cd_turma_escola AS codigo,
                                    tur.cd_tipo_turma AS tipoTurma,
				                    tur.dt_inicio_turma AS dataInicioTurma,
				                    tur.dt_fim AS dataFimTurma,
				                    tur.an_letivo AS anoletivo,				                    
				                    tur.dc_turma_escola AS nometurma,
									tur.dt_atualizacao_tabela as DataAtualizacao,
									tur.dt_status_turma_escola as DataStatusTurmaEscola,
				                    Iif(tur.cd_tipo_turma IN (1, 5, 6), se.sg_resumida_serie, 
				                    CASE
					                    WHEN tur.cd_tipo_turma = 2 THEN SUBSTRING(TUR.dc_turma_escola, 1, 1) 
					                    WHEN tur.cd_tipo_turma = 7 THEN '2' -- 2ª serie novo EM
					                    WHEN tur.cd_tipo_turma = 3 THEN '0' -- Turma de Programa
				                    END) AS ano,
				                    dtt.qt_hora_duracao AS duracaoturno,
				                    tur.cd_tipo_turno AS tipoturno, 			
				                    Iif((se.cd_etapa_ensino = 13) AND (se.cd_modalidade_ensino = 2) , 1, 0) AS ensinoEspecial,
				                    se.dc_serie_ensino AS serieEnsino,
				                    IIf(tur.st_turma_escola = 'E', 1, 0) Extinta,
				                    tur.st_turma_escola AS situacao,
                                    ee.cd_etapa_ensino as EtapaEnsino,
                                    se.cd_ciclo_ensino as CicloEnsino,
                                    esc.tp_escola as TipoEscola
				                    FROM turma_escola (nolock) tur
					                    INNER JOIN tipo_turno (nolock) t_trn
						                    ON t_trn.cd_tipo_turno = tur.cd_tipo_turno
					                    INNER JOIN duracao_tipo_turno (nolock) dtt
						                    ON t_trn.cd_tipo_turno = dtt.cd_tipo_turno
					                    AND tur.cd_duracao = dtt.cd_duracao
					                    INNER JOIN tipo_periodicidade (nolock) tper
						                    ON tur.cd_tipo_periodicidade = tper.cd_tipo_periodicidade
							                       -- Unidades Educacionais
					                    INNER JOIN v_cadastro_unidade_educacao (nolock) vue
						                    ON vue.cd_unidade_educacao = tur.cd_escola
					                    INNER JOIN escola (nolock) esc
						                    ON esc.cd_escola = vue.cd_unidade_educacao
					                    -- Serie Ensino(turma tipo = 1)
					                    LEFT JOIN  serie_turma_escola (nolock) ste
						                    ON tur.cd_turma_escola = ste.cd_turma_escola
							                    AND        ste.dt_fim IS NULL
					                    LEFT JOIN serie_ensino (nolock) se
						                    ON         se.cd_serie_ensino = ste.cd_serie_ensino
					                    LEFT JOIN  etapa_ensino (nolock) ee
						                    ON se.cd_etapa_ensino = ee.cd_etapa_ensino
											LEFT JOIN  serie_turma_grade (nolock) stg
								ON tur.cd_turma_escola = stg.cd_turma_escola
									AND tur.cd_escola = stg.cd_escola
									AND ste.cd_serie_ensino = stg.cd_serie_ensino
									AND stg.dt_fim IS NULL									
				                    WHERE tur.cd_turma_escola IN @turmaCodigos";

            using (var conexao = new SqlConnection((variaveisAmbiente.ConnectionStringEol)))
            {
                return await conexao.QueryAsync<Turma>(query, new { turmaCodigos }, commandTimeout: 120);
            }
        }

        public async Task<IEnumerable<Turma>> ObterTurmasPorCodigos(string[] codigos)
        {
            var query = @"select t.turma_id Codigo, t.nome, 
			                t.modalidade_codigo  ModalidadeCodigo, t.semestre, t.ano, t.ano_letivo AnoLetivo, tc.descricao Ciclo, t.etapa_eja EtapaEJA, t.tipo_turma TipoTurma,
			                ue.id, ue.ue_id Codigo, ue.nome, ue.tipo_escola TipoEscola,		
			                dre.id, dre.dre_id Codigo, dre.abreviacao, dre.nome
			                from  turma t
			                inner join ue on ue.id = t.ue_id 
			                inner join dre on ue.dre_id = dre.id 
                            left join tipo_ciclo_ano tca on t.modalidade_codigo = tca.modalidade and t.ano = tca.ano
                            left join tipo_ciclo tc on tca.tipo_ciclo_id = tc.id
			                where t.turma_id = ANY(@codigos)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return (await conexao.QueryAsync<Turma, Ue, Dre, Turma>(query, (turma, ue, dre) =>
                {
                    turma.Dre = dre;
                    turma.Ue = ue;

                    return turma;
                }
                , new { codigos }, splitOn: "Codigo,Id,Id"));
            }
        }


        public async Task<IEnumerable<Turma>> ObterTurmasPorCodigosSituacaoConsolidado(string[] codigos, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres)
        {
            var query = new StringBuilder();
            query.Append(@"select t.turma_id as Codigo
                            , t.nome
                            , t.modalidade_codigo  ModalidadeCodigo
                            , t.semestre
                            , t.ano
                            , t.ano_letivo AnoLetivo
                        from turma t
                       where t.turma_id = ANY(@codigos)");
            var querySituacao = new StringBuilder();
            if (situacaoFechamento.HasValue)
            {
                querySituacao.AppendLine(@"and t.id in (select turma_id from consolidado_fechamento_componente_turma 
                                   where not excluido and turma_id = t.id and status = @situacaoFechamento  ");

                if (bimestres != null && bimestres.Any())
                    querySituacao.AppendLine("and bimestre = ANY(@bimestres)");

                querySituacao.AppendLine(")");
            }

            if (situacaoConselhoClasse.HasValue)
            {
                querySituacao.AppendLine(@"and t.id in (select turma_id from consolidado_conselho_classe_aluno_turma 
                                   where not excluido and turma_id = t.id and status = @situacaoConselhoClasse  ");


                if (bimestres != null && bimestres.Any())
                    querySituacao.AppendLine("and bimestre = ANY(@bimestres)");

                querySituacao.AppendLine(")");
            }

            query.AppendLine(querySituacao.ToString());


            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<Turma>(query.ToString(), new { codigos, situacaoFechamento, situacaoConselhoClasse, bimestres });

        }


        public async Task<IEnumerable<Turma>> ObterPorAbrangenciaTiposFiltros(string codigoUe, string login, Guid perfil, Modalidade modalidade, int[] tipos, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres, int semestre = 0, bool consideraHistorico = false, int anoLetivo = 0, bool? possuiFechamento = null, bool? somenteEscolarizada = null, string codigoDre = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"select ano, anoLetivo, codigo, 
								codigoModalidade modalidadeCodigo, nome, semestre 
							from f_abrangencia_turmas_tipos(@login, @perfil, @consideraHistorico, @modalidade, @semestre, @codigoUe, @anoLetivo, @tipos) t
                            where 1=1    ");


            if (possuiFechamento.HasValue)
                query.Append(@" and codigo in (select t.turma_id from fechamento_turma ft
                                 inner join turma t on ft.turma_id = t.id
                                 where not ft.excluido)");

            if (somenteEscolarizada.HasValue && somenteEscolarizada.Value)
                query.Append(" and ano != '0'");


            if (!string.IsNullOrEmpty(codigoDre))
                query.Append(@" and codigo in (select t.turma_id from turma t
                                 inner join ue on ue.id = t.ue_id
                                 inner join dre on dre.id = ue.dre_id
                                 where dre.dre_id = @codigoDre)");


            var querySituacao = new StringBuilder();
            if (situacaoFechamento.HasValue)
            {
                querySituacao.AppendLine(@"and t.turma_id in (select turma_id from consolidado_fechamento_componente_turma 
                                   where not excluido and turma_id =  t.turma_id and status = @situacaoFechamento  ");

                if (bimestres != null && bimestres.Any())
                    querySituacao.AppendLine("and bimestre = ANY(@bimestres)");

                querySituacao.AppendLine(")");
            }

            if (situacaoConselhoClasse.HasValue)
            {
                querySituacao.AppendLine(@"and  t.turma_id in (select turma_id from consolidado_conselho_classe_aluno_turma 
                                   where not excluido and turma_id =  t.turma_id and status = @situacaoConselhoClasse  ");


                if (bimestres != null && bimestres.Any())
                    querySituacao.AppendLine("and bimestre = ANY(@bimestres)");

                querySituacao.AppendLine(")");
            }

            query.Append(querySituacao);

            var parametros = new
            {
                CodigoDre = codigoDre,
                CodigoUe = codigoUe,
                Modalidade = (int)modalidade,
                Tipos = tipos,
                AnoLetivo = anoLetivo,
                Semestre = semestre,
                Login = login,
                Perfil = perfil,
                ConsideraHistorico = consideraHistorico,
                SituacaoFechamento = situacaoFechamento,
                SituacaoConselhoClasse = situacaoConselhoClasse,
                Bimestres = bimestres
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<Turma>(query.ToString(), parametros);

        }

        public async Task<DreUe> ObterDreUePorTurmaId(long turmaId)
        {
            var query = TurmaConsultas.DadosCompletosDreUePorTurmaId;
            var parametros = new { turmaId };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryFirstOrDefaultAsync<DreUe>(query, parametros);
        }

        public async Task<IEnumerable<Aluno>> ObterDadosAlunosPorTurmaDataMatricula(string codigoTurma, DateTime dataMatricula)
        {
            var query = TurmaConsultas.DadosAlunosDataMatricula;
            var parametros = new { CodigoTurma = codigoTurma, dataMatricula };

            using var conexao = new SqlConnection(variaveisAmbiente.ConnectionStringEol);
            return await conexao.QueryAsync<Aluno>(query, parametros);
        }

        public async Task<IEnumerable<TurmaItinerarioEnsinoMedioDto>> ObterTurmasItinerarioEnsinoMedio()
        {
            var query = @"select id, nome, serie from turma_tipo_itinerario tti";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol);
            return await conexao.QueryAsync<TurmaItinerarioEnsinoMedioDto>(query);
        }
    }
}
