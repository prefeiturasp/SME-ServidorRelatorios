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
    }
}
