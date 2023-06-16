using Dapper;
using Elastic.Apm.Api;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseAlunoRepository : IConselhoClasseAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ParecerConclusivo;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QuerySingleOrDefaultAsync<string>(query, parametros);
            }
        }

        public async Task<IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto>> ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsAsync(long[] conselhoClasseAlunoIds)
        {
            var query = @"select
	                        cca.aluno_codigo as AlunoCodigo,
	                        cca.anotacoes_pedagogicas as AnotacaoPedagogica
                        from
	                        conselho_classe_aluno cca
                        where
	                        cca.id =  ANY(@conselhoClasseAlunoIds)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<AnotacoesPedagogicasAlunoIdsQueryDto>(query, new { conselhoClasseAlunoIds });
            }
        }

        public async Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterParecerConclusivoPorTurma(string turmaCodigo)
        {
            var query = ConselhoClasseAlunoConsultas.ParecerConclusivoPorTurma;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<ConselhoClasseParecerConclusivo>(query, new { turmaCodigo });
            }
        }

        public async Task<RecomendacoesConselhoClasse> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.Recomendacoes;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                var lookup = new Dictionary<long, RecomendacoesConselhoClasse>();

                await conexao.QueryAsync<RecomendacoesConselhoClasse, ConselhoClasseRecomendacao, Dictionary< long, RecomendacoesConselhoClasse > >
                                            (query.ToString(), (conselhoClasse, ccRecomendacao) =>
                                            {

                                                RecomendacoesConselhoClasse recomendacoesConselhoClasse;

                                                if (!lookup.TryGetValue(conselhoClasse.ConselhoClasseAlunoId, out recomendacoesConselhoClasse))
                                                {
                                                    recomendacoesConselhoClasse = conselhoClasse;
                                                    lookup.Add(conselhoClasse.ConselhoClasseAlunoId, recomendacoesConselhoClasse);
                                                }

                                                if (ccRecomendacao != null)
                                                {
                                                    if (ccRecomendacao.Tipo == ConselhoClasseRecomendacaoTipo.Familia)
                                                        recomendacoesConselhoClasse.RecomendacoesPreDefinidasFamilia.Add(ccRecomendacao);
                                                    else
                                                    if (ccRecomendacao.Tipo == ConselhoClasseRecomendacaoTipo.Aluno)
                                                        recomendacoesConselhoClasse.RecomendacoesPreDefinidasAluno.Add(ccRecomendacao);
                                                }

                                                return lookup;
                                            }, parametros);

                return lookup.Values.FirstOrDefault();

            }
        }

        public async Task<IEnumerable<RecomendacaoConselhoClasseAluno>> ObterRecomendacoesPorAlunosTurmas(string[] codigosAluno, string[] codigosTurma, int anoLetivo, Modalidade? modalidade, int semestre)
        {
            var query = new StringBuilder(@"select cc.id, t.turma_id TurmaCodigo, cca.aluno_codigo AlunoCodigo,
                                 cca.recomendacoes_aluno RecomendacoesAluno, 
                                 cca.recomendacoes_familia RecomendacoesFamilia,
                                 cca.anotacoes_pedagogicas AnotacoesPedagogicas
                                from conselho_classe cc
                                inner join fechamento_turma ft on cc.fechamento_turma_id = ft.id 
                                inner join turma t on ft.turma_id = t.id 
                                inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id
                                inner join tipo_calendario tc on t.ano_letivo = tc.ano_letivo and tc.modalidade = @modalidadeTipoCalendario
                                inner join periodo_escolar p on p.tipo_calendario_id = tc.id
                                where 1 = 1");

            if (codigosAluno != null && codigosAluno.Length > 0)
                query.AppendLine(" and cca.aluno_codigo = ANY(@codigosAluno) ");

            if (codigosTurma != null && codigosTurma.Length > 0)
                query.AppendLine(" and t.turma_id = ANY(@codigosTurma) ");

            if (anoLetivo > 0)
                query.AppendLine(" and t.ano_letivo = @anoLetivo ");

            if (modalidade.HasValue)
                query.AppendLine(" and t.modalidade_codigo = @modalidade ");

            DateTime dataReferencia = DateTime.MinValue;
            if (modalidade == Modalidade.EJA)
            {
                var periodoReferencia = semestre == 1 ? "periodo_inicio < @dataReferencia" : "periodo_fim > @dataReferencia";
                query.AppendLine($"and exists(select 0 from periodo_escolar p where tipo_calendario_id = tc.id and {periodoReferencia})");

                // 1/6/ano ou 1/7/ano dependendo do semestre
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 7, 1);
            }

            query.Append(@" group by (cc.id, t.turma_id, cca.aluno_codigo,
                                 cca.recomendacoes_aluno,
                                 cca.recomendacoes_familia,
                                 cca.anotacoes_pedagogicas,
                                 cc.alterado_em, cc.criado_em)
                            order by coalesce(cc.alterado_em, cc.criado_em) desc");

            var parametros = new
            {
                codigosAluno,
                codigosTurma,
                anoLetivo,
                modalidade = (int)modalidade,
                modalidadeTipoCalendario = modalidade.HasValue ? (int)modalidade.Value.ObterModalidadeTipoCalendario() : (int)default,
                dataReferencia
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RecomendacaoConselhoClasseAluno>(query.ToString(), parametros);
            }
        }

        public async Task<bool> PossuiConselhoClasseCadastrado(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ObterPorConselhoClasseId;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QuerySingleOrDefaultAsync<bool>(query, parametros);
            }
        }


        public async Task<IEnumerable<RecomendacoesAlunoFamiliaDto>> ObterRecomendacoesAlunoFamiliaPorAlunoEFechamentoTurma(long fechamentoTurmaId, string codigoAluno)
        {
            string sql = @"select distinct ccr.recomendacao, ccr.tipo from conselho_classe_aluno_recomendacao ccar
                                 inner join conselho_classe_recomendacao ccr on ccr.id = ccar.conselho_classe_recomendacao_id
                                 inner join conselho_classe_aluno cca on cca.id = ccar.conselho_classe_aluno_id
                                 inner join conselho_classe cc on cc.id = cca.conselho_classe_id
                                    where cca.aluno_codigo = @codigoAluno
                                    and cc.fechamento_turma_id = @fechamentoTurmaId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RecomendacoesAlunoFamiliaDto>(sql, new { codigoAluno, fechamentoTurmaId });
            }
        }

        public async Task<IEnumerable<RecomendacoesAlunoFamiliaDto>> ObterRecomendacoesAlunoFamiliaPorAlunoETurma(string codigoAluno, string codigoTurma, int id)
        {
            string sql = @"select distinct ccr.recomendacao, ccr.tipo from conselho_classe_aluno_recomendacao ccar
                                 inner join conselho_classe_recomendacao ccr on ccr.id = ccar.conselho_classe_recomendacao_id
                                 inner join conselho_classe_aluno cca on cca.id = ccar.conselho_classe_aluno_id
                                 inner join conselho_classe cc on cc.id = cca.conselho_classe_id
                                 inner join fechamento_turma ft on ft.id = cc.fechamento_turma_id
                                 inner join turma t on t.id = ft.turma_id
                                    where t.turma_id = @codigoTurma and cca.aluno_codigo = @codigoAluno
                                    and cc.id = @id";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<RecomendacoesAlunoFamiliaDto>(sql, new { codigoAluno, codigoTurma, id});
            }
        }

        public async Task<IEnumerable<ConselhoDeClasseAlunoIdDto>> ObterConselhoDeClasseAlunoId(long[] turmaIds, string[] codigosAlunos)
        {
            string sql = @" select cca.id as ConselhoClasseAlunoId, cca.aluno_codigo as AlunoCodigo, ft.turma_id as TurmaId
                            from fechamento_turma ft
                            inner join conselho_classe cc on cc.fechamento_turma_id = ft.id
                            inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id
                            where not cc.excluido 
                            and not ft.excluido 
                            and not cca.excluido 
                            and ft.turma_id = ANY(@turmaIds)
                            and cca.aluno_codigo = ANY(@codigosAlunos)";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<ConselhoDeClasseAlunoIdDto>(sql, new { turmaIds, codigosAlunos });
            }
        }
    }
}
